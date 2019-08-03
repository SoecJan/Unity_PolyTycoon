using System.Collections.Generic;
using UnityEngine;

public interface ITransportRouteManager
{
    void RemoveTransportRoute(TransportRoute transportRoute);
    void CreateTransportRoute(string routeName, TransportVehicle vehicle, List<TransportRouteElement> routeElements);
    void OnTransportRouteChange(TransportRoute transportRoute);
}

/// <summary>
/// Handles <see cref="TransportRouteCreateController"/> and <see cref="TransportRouteOverview"/> interaction.
/// </summary>
public class TransportRouteManager : MonoBehaviour, ITransportRouteManager
{
    #region Attributes

    [Header("Controller")] 
    private TransportRouteCreateController _routeCreateController;
    private ITransportRouteOverview _transportRouteOverview;
    private IVehicleManager _vehicleManager;
    private IPathFinder _pathFinder;
    private IUserInformationPopup _userPopup;

    #endregion

    #region Getter & Setter

    private TransportRouteCreateController RouteCreateController
    {
        get => _routeCreateController;

        set => _routeCreateController = value;
    }

    #endregion

    #region Methods

    void Start()
    {
        this._routeCreateController = FindObjectOfType<TransportRouteCreateController>();
        this._transportRouteOverview = FindObjectOfType<TransportRouteOverview>();
        this._vehicleManager = FindObjectOfType<VehicleManager>();
        this._userPopup = FindObjectOfType<UserInformationPopup>();
        this._pathFinder = new PathFinder(FindObjectOfType<TerrainGenerator>());
    }

    private void Reset()
    {
        RouteCreateController.Reset();
    }

    /// <summary>
    /// Removes a TransportRoute and vehicle from the game
    /// </summary>
    /// <param name="transportRoute">The transport route to be removed</param>
    public void RemoveTransportRoute(TransportRoute transportRoute)
    {
        Destroy(transportRoute.Vehicle.gameObject);
        _transportRouteOverview.Remove(transportRoute);
    }

    public void CreateTransportRoute(string routeName, TransportVehicle vehicle,
        List<TransportRouteElement> routeElements)
    {
        TransportRoute transportRoute = new TransportRoute
        {
            Vehicle = vehicle,
            RouteName = routeName,
            TransportRouteElements = routeElements
        };
        ThreadedDataRequester.RequestData(() => _pathFinder.FindPath(transportRoute), FoundPath);
    }

    private void FoundPath(object result)
    {
        TransportRoute transportRoute = (TransportRoute) result;
        OnTransportRoutePathFound(transportRoute);
    }

    public void OnTransportRouteChange(TransportRoute transportRoute)
    {
//        PrintNodes(transportRoute);
//		_networkPathFinder.FindPath(transportRoute);
        Debug.LogError("TransportRouteChanges need to be reimplemented"); // TODO
    }

    private void OnTransportRouteChangePathFound(TransportRoute transportRoute)
    {
        Debug.Log("Route updated");
        transportRoute.Vehicle.TransportRoute = transportRoute;
        RouteCreateController.SetVisible(false);
        Reset();
    }

    private void OnTransportRoutePathFound(TransportRoute transportRoute)
    {
        if (transportRoute.Vehicle == null)
        {
            _userPopup.InformationText = "Choose a vehicle";
            return;
        }

        if (transportRoute.TransportRouteElements == null || transportRoute.TransportRouteElements.Count <= 1)
        {
            _userPopup.InformationText = "Not enough stations";
            return;
        }

        if ("".Equals(transportRoute.RouteName))
        {
            _userPopup.InformationText = "Route needs to have a name";
            return;
        }

        foreach (TransportRouteElement element in transportRoute.TransportRouteElements)
        {
            if (element.Path != null) continue;
            _userPopup.InformationText = "Stations are not connected";
            return;
        }

        // Add TransportRoute to Overview
        _transportRouteOverview.Add(transportRoute);
        // Instantiate and configure Vehicle
        GameObject instancedVehicle = _vehicleManager.AddVehicle(transportRoute.Vehicle,
            transportRoute.TransportRouteElements[0].Path.WayPoints[0].TraversalVectors[0]);
        TransportVehicle transportVehicle = instancedVehicle.GetComponent<TransportVehicle>();
        transportRoute.Vehicle = transportVehicle;
        transportVehicle.TransportRoute = transportRoute;
        // Reset Route UI
        RouteCreateController.SetVisible(false);
        Reset();
    }

    #endregion
}