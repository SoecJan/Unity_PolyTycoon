using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransportRouteCreateController : AbstractUi
{
    #region Attributes

    private TransportRoute _selectedTransportRoute;
    private IUserInformationPopup _userInformationPopup;

    [Header("Navigation")] 
    [SerializeField] private Button _showButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Toggle _vehicleToggle;
    [SerializeField] private Toggle _routeToggle;
    [SerializeField] private Button _createButton;
    [SerializeField] private Button _applyButton;
    [Header("Controller")]
    [SerializeField] private TransportRouteManager _transportRouteManager;
    [SerializeField] private RouteCreationVehicleManager _vehicleManager;
    [SerializeField] private RouteCreationStationManager _stationManager;
    [SerializeField] private RouteCreationSettingsManager _settingsController;

    [SerializeField] public RouteCreationSettingsManager SettingController => _settingsController;
    [SerializeField] public RouteCreationStationManager StationManager => _stationManager;
    [SerializeField] public RouteCreationVehicleManager VehicleManager => _vehicleManager;

    public Toggle RouteToggle => _routeToggle;

    public IUserInformationPopup UserInformationPopup => _userInformationPopup;

    #endregion

    #region Standard Methods

    private void Start()
    {
        _userInformationPopup = FindObjectOfType<UserInformationPopup>();
        _vehicleManager.Initialize();
        _vehicleManager.OnVehicleSelect += vehicle => _routeToggle.interactable = vehicle;
        SettingController.Initialize();
        _stationManager.RouteCreateController = this;

        _showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf); });
        _exitButton.onClick.AddListener(delegate
        {
            SetVisible(false);
            Reset();
        });

        _routeToggle.onValueChanged.AddListener(delegate(bool value)
        {
            _vehicleManager.VehicleChoiceVisibleGameObject.SetActive(!value);
            _stationManager.RouteElementVisibleGameObject.SetActive(value);
            if (value)
            {
                SettingController.OnShow(_vehicleManager.SelectedVehicle);
            }
            else
            {
                SettingController.Reset();
            }
        });
        _createButton.onClick.AddListener(CreateRoute);
        _applyButton.onClick.AddListener(SaveRouteChanges);
    }

    public new void Reset()
    {
        _selectedTransportRoute = null;
        _createButton.gameObject.SetActive(true);
        _applyButton.gameObject.SetActive(false);
        _vehicleToggle.isOn = true;
        _routeToggle.isOn = false;
        _routeToggle.interactable = false;
        _createButton.interactable = false;
        _vehicleManager.Reset();
        SettingController.Reset();
        _stationManager.Reset();
    }

    public override void OnShortCut()
    {
        SetVisible(true);
    }

    #endregion

    public void IsReady()
    {
        if (!_vehicleManager.SelectedVehicle) return;
        if (_stationManager.TransportRouteElementViews.Count <= 1) return;
        _createButton.interactable = true;
    }

    public void CreateRoute()
    {
        if (!_vehicleManager.SelectedVehicle)
            _userInformationPopup.InformationText = "Vehicle needs to be set first!";
        if (_stationManager.TransportRouteElementViews.Count <= 1)
            _userInformationPopup.InformationText = "A route needs more than 1 station!";

        string routeName = _stationManager.RouteName;
        TransportVehicle transportVehicle = _vehicleManager.SelectedVehicle;
        List<TransportRouteElement> routeElements = _stationManager.TransportRouteElements;
            
        _transportRouteManager.CreateTransportRoute(routeName, transportVehicle, routeElements);
    }
    
    public void LoadRoute(TransportRoute transportRoute)
    {
        _selectedTransportRoute = transportRoute;
        _createButton.gameObject.SetActive(false);
        _applyButton.gameObject.SetActive(true);
        SetVisible(true);
        VehicleManager.SelectedVehicle = _selectedTransportRoute.Vehicle;
        StationManager.LoadTransportRoute(_selectedTransportRoute);
    }

    public void SaveRouteChanges()
    {
        if (!_vehicleManager.SelectedVehicle)
            _userInformationPopup.InformationText = "Vehicle needs to be set first!";
        if (_stationManager.TransportRouteElementViews.Count <= 1)
            _userInformationPopup.InformationText = "A route needs more than 1 station!";

        _selectedTransportRoute.Vehicle = _vehicleManager.SelectedVehicle;
        _selectedTransportRoute.RouteName = _stationManager.RouteName;
        _selectedTransportRoute.TransportRouteElements = _stationManager.TransportRouteElements;
        _transportRouteManager.OnTransportRouteChange(_selectedTransportRoute);
    }

    public override void SetVisible(bool visible)
    {
        base.SetVisible(visible);
        if (visible)
        {
            _stationManager.RouteName = "Route #" + TransportRoute.RouteIndex;
        }
    }
}