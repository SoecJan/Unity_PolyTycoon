using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles <see cref="TransportRouteCreationView"/> and <see cref="TransportRouteOverviewView"/> interaction.
/// </summary>
public class TransportRouteManager : ITransportRouteManager
{
    #region Attributes

    [Header("Controller")] 
    private TransportRouteCreationView _routeCreationView;
    private ITransportRouteOverview _transportRouteOverview;
    private IVehicleManager _vehicleManager;
    private IPathFinder _pathFinder;
    private IUserNotificationView _userPopup;

    #endregion

    #region Getter & Setter

    private TransportRouteCreationView RouteCreationView
    {
        get => _routeCreationView;

        set => _routeCreationView = value;
    }

    #endregion

    #region Methods

    public TransportRouteManager(PathFinder pathFinder, IVehicleManager vehicleManager)
    {
        this._vehicleManager = vehicleManager;
        this._pathFinder = pathFinder;
        
        this._routeCreationView = GameObject.FindObjectOfType<TransportRouteCreationView>();
        this._transportRouteOverview = GameObject.FindObjectOfType<TransportRouteOverviewView>();
        this._userPopup = GameObject.FindObjectOfType<UserNotificationView>();
    }

    private void Reset()
    {
        RouteCreationView.Reset();
    }

    /// <summary>
    /// Removes a TransportRoute and vehicle from the game
    /// </summary>
    /// <param name="transportRoute">The transport route to be removed</param>
    public void RemoveTransportRoute(TransportRoute transportRoute)
    {
        GameObject.Destroy(transportRoute.TransportVehicle.gameObject);
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
        RouteCreationView.SetVisible(false);
        Reset();
    }

    private void OnTransportRoutePathFound(List<TransportRouteElement> transportRouteElements)
    {
        TransportVehicleData transportVehicleData = _routeCreationView.TransportVehicleData;
        if (transportVehicleData == null)
        {
            _userPopup.InformationText = "Choose a vehicle";
            return;
        }
        
        if ("".Equals(_routeCreationView.RouteName))
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
            RouteName = _routeCreationView.RouteName,
            TransportVehicle = transportVehicle
        };

        // Configure Vehicle
        transportVehicle.TransportRoute = transportRoute;
        // Add TransportRoute to Overview
        _transportRouteOverview.Add(transportRoute);
        // Reset Route UI
        RouteCreationView.SetVisible(false);
        Reset();
    }

    #endregion
}