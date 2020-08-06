
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class NewRouteController
{
    private NewRouteView _view;
    private ITransportRouteManager _transportRouteManager;
    private NewRouteStationController _stationController;
    private NewRouteVehicleChoiceView _vehicleChoice;
    private IUserNotificationView _userNotification;
    private IPathFinder _pathFinder;
    
    public NewRouteController(ITransportRouteManager transportRouteManager, 
        IPathFinder pathFinder, IVehicleManager vehicleManager, 
        IUserNotificationView _userNotification, NewRouteView newRouteView)
    {
        this._view = newRouteView;
        this._view.OnHide += Reset;
        this._transportRouteManager = transportRouteManager;
        this._userNotification = _userNotification;
        this._pathFinder = pathFinder;

        _vehicleChoice = newRouteView.GetComponent<NewRouteVehicleChoiceView>();
        _vehicleChoice.Vehicles = vehicleManager.VehicleList;

        _stationController = newRouteView.GetComponent<NewRouteStationController>();
        _stationController.PathFinder = pathFinder;
        _stationController.VehicleChoiceController = _vehicleChoice;
        
        this.View.RouteNameInputfield.onValueChanged.AddListener(delegate(string name) { RouteName = name; });
        this.View.CreateButton.onClick.AddListener(Create);
    }

    public TransportVehicleData TransportVehicleData { get; set; }
    public string RouteName { get; set; }

    public NewRouteView View => _view;

    private void Create()
    {
        TransportVehicleData transportVehicleData = _vehicleChoice.SelectedVehicle;
        List<TransportRouteElement> routeElements = _stationController.TransportRouteElements;

        
        if (!transportVehicleData)
        {
            _userNotification.InformationText = "Vehicle needs to be set first!";
            return;
        } 
        if (routeElements == null || routeElements.Count <= 1)
        {
            _userNotification.InformationText = "A route needs more than 1 station!";
            return;
        }
        if ("".Equals(RouteName) || RouteName == null)
        {
            string generatedRouteName = "Generated Route";
            // string generatedRouteName = routeElements[0].FromNode.name + " (" + routeElements[0].RouteSettings[0].ProductData.ProductName + ")";
            _userNotification.InformationText = "Forgot Route Name. Route was added as: " + generatedRouteName;
        }

        _transportRouteManager.CreateTransportRoute(transportVehicleData, routeElements);
    }

    public void Reset()
    {
        RouteName = "";
        _view.Reset();
        _stationController.Reset();
        _vehicleChoice.Reset();
    }

    public void OnStationClick(PathFindingTarget target)
    {
        _stationController.OnStationClick(target);
    }
}
