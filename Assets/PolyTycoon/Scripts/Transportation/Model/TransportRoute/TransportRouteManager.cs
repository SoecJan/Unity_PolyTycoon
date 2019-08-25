using System.Collections.Generic;
using UnityEngine;

public interface ITransportRouteManager
{
    void RemoveTransportRoute(TransportRoute transportRoute);
    void CreateTransportRoute(TransportVehicleData transportVehicleData, List<TransportRouteElement> transportRouteElements);
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
        Destroy(transportRoute.TransportVehicle.gameObject);
        _transportRouteOverview.Remove(transportRoute);
    }

    public void CreateTransportRoute(TransportVehicleData transportVehicleData,
        List<TransportRouteElement> routeElements)
    {
        ThreadedDataRequester.RequestData(() => _pathFinder.FindPath(transportVehicleData, routeElements), FoundPath);
    }

    private void FoundPath(object result)
    {
        List<TransportRouteElement> transportRouteElements = (List<TransportRouteElement>) result;
        OnTransportRoutePathFound(transportRouteElements);
    }

    private void OnTransportRouteChangePathFound(TransportRoute transportRoute)
    {
        Debug.Log("Route updated");
        transportRoute.TransportVehicle.TransportRoute = transportRoute;
        RouteCreateController.SetVisible(false);
        Reset();
    }

    private void OnTransportRoutePathFound(List<TransportRouteElement> transportRouteElements)
    {
        TransportVehicleData transportVehicleData = _routeCreateController.TransportVehicleData;
        if (transportVehicleData == null)
        {
            _userPopup.InformationText = "Choose a vehicle";
            return;
        }
        
        if ("".Equals(_routeCreateController.RouteName))
        {
            _userPopup.InformationText = "Route needs to have a name";
            return;
        }

        if (transportRouteElements == null || transportRouteElements.Count <= 1)
        {
            _userPopup.InformationText = "Not enough stations";
            return;
        }

        foreach (TransportRouteElement element in transportRouteElements)
        {
            if (element.Path != null) continue;
            _userPopup.InformationText = "Stations are not connected";
            return;
        }

        Vector3 startPosition = transportRouteElements[0].Path.WayPoints[0].TraversalVectors[0];
        TransportVehicle transportVehicle = _vehicleManager.AddVehicle(transportVehicleData, startPosition);
        // Initialize TransportRoute
        TransportRoute transportRoute = new TransportRoute
        {
            TransportRouteElements = transportRouteElements,
            RouteName = _routeCreateController.RouteName,
            TransportVehicle = transportVehicle
        };

        // Configure Vehicle
        transportVehicle.TransportRoute = transportRoute;
        // Add TransportRoute to Overview
        _transportRouteOverview.Add(transportRoute);
        // Reset Route UI
        RouteCreateController.SetVisible(false);
        Reset();
    }

    #endregion
}