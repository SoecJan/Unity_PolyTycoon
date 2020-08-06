using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles <see cref="TransportRouteCreationView"/> and <see cref="TransportRouteOverviewView"/> interaction.
/// </summary>
public class TransportRouteManager : ITransportRouteManager
{
    #region Attributes

    [Header("Controller")] 
    private NewRouteController _newRouteController;
    private RouteOverviewController _routeOverviewController;
    private IVehicleManager _vehicleManager;
    private IPathFinder _pathFinder;
    private IUserNotificationView _userPopup;

    private Dictionary<PathType, List<TransportRoute>> _transportRouteDictionary;

    #endregion

    #region Getter & Setter

    public NewRouteController RouteCreationController
    {
        get => _newRouteController;

        set => _newRouteController = value;
    }

    #endregion

    #region Methods

    public TransportRouteManager(PathFinder pathFinder, IVehicleManager vehicleManager)
    {
        this._vehicleManager = vehicleManager;
        this._pathFinder = pathFinder;
        _transportRouteDictionary = new Dictionary<PathType, List<TransportRoute>>();

        RouteOverviewView overviewView = GameObject.FindObjectOfType<RouteOverviewView>();
        _routeOverviewController = new RouteOverviewController(this, overviewView);

        NewRouteView _routeCreationView = GameObject.FindObjectOfType<NewRouteView>();
        this._userPopup = GameObject.FindObjectOfType<UserNotificationView>();
        _newRouteController = new NewRouteController(this, pathFinder, vehicleManager, _userPopup, _routeCreationView);

        
        
        // this._transportRouteOverview = GameObject.FindObjectOfType<TransportRouteOverviewView>();
        
    }

    private void Reset()
    {
        _newRouteController.Reset();
    }

    /// <summary>
    /// Removes a TransportRoute and vehicle from the game
    /// </summary>
    /// <param name="transportRoute">The transport route to be removed</param>
    public void RemoveTransportRoute(TransportRoute transportRoute)
    {
        foreach (TransportVehicle transportVehicle in transportRoute.TransportVehicles)
        {
            GameObject.Destroy(transportVehicle.gameObject);
        }

        PathType pathType = transportRoute.TransportVehicles[0].RouteMover.PathType;
        bool success = _transportRouteDictionary[pathType].Remove(transportRoute);
        Debug.Log(success);
    }

    public void CreateTransportRoute(TransportVehicleData transportVehicleData,
        List<TransportRouteElement> routeElements)
    {
        if (transportVehicleData == null)
        {
            _userPopup.InformationText = "Choose a vehicle";
            return;
        }
        
        if ("".Equals(_newRouteController.RouteName))
        {
            _userPopup.InformationText = "Route needs to have a name";
            return;
        }

        if (routeElements == null || routeElements.Count <= 1)
        {
            _userPopup.InformationText = "Not enough stations";
            return;
        }

        foreach (TransportRouteElement element in routeElements)
        {
            if (element.Path != null) continue;
            _userPopup.InformationText = "Stations are not connected";
            return;
        }

        Vector3 startPosition = routeElements[0].Path.WayPoints[0].TraversalVectors[0];
        TransportVehicle transportVehicle = _vehicleManager.AddVehicle(transportVehicleData, startPosition);
        // Initialize TransportRoute
        TransportRoute transportRoute = new TransportRoute
        {
            TransportRouteElements = routeElements,
            RouteName = _newRouteController.RouteName,
            TransportVehicles = new List<TransportVehicle>() {transportVehicle}
        };

        // Configure Vehicle
        transportVehicle.TransportRoute = transportRoute;
        // Add TransportRoute to Overview
        PathType pathType = transportRoute.TransportVehicles[0].RouteMover.PathType;
        if (_transportRouteDictionary.ContainsKey(pathType))
        { 
            _transportRouteDictionary[pathType].Add(transportRoute);
        }
        else
        {
            _transportRouteDictionary.Add(pathType, new List<TransportRoute>() {transportRoute});
        }
        // Reset Route UI
        _newRouteController.View.SetVisible(false);
        Reset();
    }

    public void EditTransportRoute(TransportRoute transportRoute)
    {
        Debug.Log("Edit Route " + transportRoute.RouteName);
    }

    public void DuplicateTransportRoute(TransportRoute transportRoute)
    {
        Debug.Log("Duplicate Route " + transportRoute.RouteName);
    }

    public List<TransportRoute> GetRoutes(PathType pathType)
    {
        switch (pathType)
        {
            case PathType.Road:
                return _transportRouteDictionary.ContainsKey(pathType) ? _transportRouteDictionary[PathType.Road] : null;
            case PathType.Rail:
                return _transportRouteDictionary.ContainsKey(pathType) ? _transportRouteDictionary[PathType.Rail] : null;
            case PathType.Air:
                return _transportRouteDictionary.ContainsKey(pathType) ? _transportRouteDictionary[PathType.Air] : null;
            case PathType.Water:
                return _transportRouteDictionary.ContainsKey(pathType) ? _transportRouteDictionary[PathType.Water] : null;
            default:
                return _transportRouteDictionary.SelectMany(d => d.Value).ToList();
        }
    }

    #endregion
}