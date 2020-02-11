using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransportRouteCreationView : AbstractUi
{
    #region Attributes

    private TransportRoute _selectedTransportRoute;
    private IUserNotificationView _userNotificationView;

    [Header("Navigation")] 
    [SerializeField] private Button _showButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Toggle _vehicleToggle;
    [SerializeField] private Toggle _routeToggle;
    [SerializeField] private Button _createButton;
    [SerializeField] private Button _applyButton;
    [Header("Controller")]
    [SerializeField] private TransportRouteManager _transportRouteManager;
    [SerializeField] private RouteCreationVehicleChoiceSubView vehicleChoiceSubView;
    [SerializeField] private RouteCreationStationManager _stationManager;
    [SerializeField] private RouteCreationSettingsManager _settingsController;

    [SerializeField] public RouteCreationSettingsManager SettingController => _settingsController;
    [SerializeField] public RouteCreationStationManager StationManager => _stationManager;
    [SerializeField] public RouteCreationVehicleChoiceSubView VehicleChoiceSubView => vehicleChoiceSubView;

    public Toggle RouteToggle => _routeToggle;

    public IUserNotificationView UserNotificationView => _userNotificationView;

    #endregion

    #region Standard Methods

    private void Start()
    {
        _transportRouteManager = FindObjectOfType<GameHandler>().TransportRouteManager;
        _userNotificationView = FindObjectOfType<UserNotificationView>();
        vehicleChoiceSubView.Initialize();
        vehicleChoiceSubView.OnVehicleSelect += transportVehicleData => _routeToggle.interactable = transportVehicleData;
        SettingController.Initialize();
        _stationManager.RouteCreationView = this;

        _showButton.onClick.AddListener(delegate { SetVisible(true); });
        _exitButton.onClick.AddListener(delegate
        {
            SetVisible(false);
            Reset();
        });

        _routeToggle.onValueChanged.AddListener(delegate(bool value)
        {
            vehicleChoiceSubView.VehicleChoiceVisibleGameObject.SetActive(!value);
            _stationManager.RouteElementVisibleGameObject.SetActive(value);
            if (value)
            {
                TransportVehicleData transportVehicleData = vehicleChoiceSubView.SelectedTransportVehicleData;
                SettingController.OnShow(transportVehicleData != null ? transportVehicleData.MaxCapacity : -1);
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
        _vehicleToggle.interactable = true;
        _vehicleToggle.isOn = true;
        _routeToggle.isOn = false;
        _routeToggle.interactable = false;
        _createButton.interactable = false;
        vehicleChoiceSubView.Reset();
        SettingController.Reset();
        _stationManager.Reset();
    }

    public override void OnShortCut()
    {
        SetVisible(true);
    }
    
    public string RouteName { get => _stationManager.RouteName; }
    
    public TransportVehicleData TransportVehicleData { get => vehicleChoiceSubView.SelectedTransportVehicleData;}
    
    #endregion

    public void IsReady()
    {
        if (!vehicleChoiceSubView.SelectedTransportVehicleData) return;
        if (_stationManager.TransportRouteElementViews.Count <= 1) return;
        _createButton.interactable = true;
    }

    public void CreateRoute()
    {
        if (!TransportVehicleData)
            _userNotificationView.InformationText = "Vehicle needs to be set first!";
        if (_stationManager.TransportRouteElementViews.Count <= 1)
            _userNotificationView.InformationText = "A route needs more than 1 station!";

        TransportVehicleData transportVehicleData = vehicleChoiceSubView.SelectedTransportVehicleData;
        List<TransportRouteElement> routeElements = _stationManager.TransportRouteElements;
            
        _transportRouteManager.CreateTransportRoute(transportVehicleData, routeElements);
    }
    
    public void LoadRoute(TransportRoute transportRoute)
    {
        _selectedTransportRoute = transportRoute;
        vehicleChoiceSubView.VehicleChoiceVisibleGameObject.SetActive(false);
        _stationManager.RouteElementVisibleGameObject.SetActive(true);
        _vehicleToggle.isOn = false;
        _vehicleToggle.interactable = false;
        _routeToggle.interactable = true;
        _routeToggle.isOn = true;
        _createButton.gameObject.SetActive(false);
        _applyButton.gameObject.SetActive(true);
        SettingController.OnShow(_selectedTransportRoute.TransportVehicle.MaxCapacity);
        StationManager.LoadTransportRoute(_selectedTransportRoute);
        SetVisible(true);
    }

    public void SaveRouteChanges()
    {
        _selectedTransportRoute.RouteName = _stationManager.RouteName;
        _selectedTransportRoute.TransportRouteElements = _stationManager.TransportRouteElements;
        _selectedTransportRoute.TransportVehicle.TransportRoute = _selectedTransportRoute;
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