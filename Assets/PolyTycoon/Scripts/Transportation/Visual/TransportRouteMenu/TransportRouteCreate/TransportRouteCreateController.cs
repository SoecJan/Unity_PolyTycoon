using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class TransportRouteCreateController : AbstractUi
{
    #region Attributes

    private TransportRoute _selectedTransportRoute;
    private UserInformationPopup _userInformationPopup;

    [Header("Navigation")] [SerializeField]
    private Button _showButton;

    [SerializeField] private Button _exitButton;
    [SerializeField] private Toggle _vehicleToggle;
    [SerializeField] private Toggle _routeToggle;
    [SerializeField] private Button _createButton;
    [SerializeField] private Button _applyButton;

    [Header("Controller")] [SerializeField]
    private TransportRouteManager _transportRouteManager;

    [SerializeField] private RouteVehicleChooser _vehicleChooser;
    [SerializeField] private RouteSettingController _settingController;
    [SerializeField] private RouteElementController _routeElementController;

    public RouteSettingController SettingController
    {
        get { return _settingController; }

        set { _settingController = value; }
    }

    public RouteElementController RouteElementController
    {
        get { return _routeElementController; }

        set { _routeElementController = value; }
    }

    public RouteVehicleChooser VehicleChooser
    {
        get { return _vehicleChooser; }

        set { _vehicleChooser = value; }
    }

    public Toggle VehicleToggle
    {
        get { return _vehicleToggle; }
    }

    public Toggle RouteToggle
    {
        get { return _routeToggle; }
    }

    public UserInformationPopup UserInformationPopup
    {
        get { return _userInformationPopup; }
        set { _userInformationPopup = value; }
    }

    #endregion

    #region Standard Methods

    private void Start()
    {
        _userInformationPopup = FindObjectOfType<UserInformationPopup>();
        _vehicleChooser.RouteCreateController = this;
        _settingController.RouteCreateController = this;
        _routeElementController.RouteCreateController = this;

        _showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf); });
        _exitButton.onClick.AddListener(delegate
        {
            SetVisible(false);
            Reset();
        });

        RouteToggle.onValueChanged.AddListener(delegate(bool value)
        {
            _vehicleChooser.VisibleGameObject.SetActive(!value);
            _routeElementController.VisibleGameObject.SetActive(value);
            if (value) SettingController.OnShow();
        });
        _createButton.onClick.AddListener(delegate
        {
            if (!_vehicleChooser.SelectedVehicle)
                _userInformationPopup.InformationText = "Vehicle needs to be set first!";
            if (_routeElementController.TransportRouteElementViews.Count <= 1)
                _userInformationPopup.InformationText = "A route needs more than 1 station!";
            _transportRouteManager.OnTransportRouteCreate();
        });
        _applyButton.onClick.AddListener(delegate
        {
            if (!_vehicleChooser.SelectedVehicle)
                _userInformationPopup.InformationText = "Vehicle needs to be set first!";
            if (_routeElementController.TransportRouteElementViews.Count <= 1)
                _userInformationPopup.InformationText = "A route needs more than 1 station!";

            _selectedTransportRoute.Vehicle = _vehicleChooser.SelectedVehicle;
            _selectedTransportRoute.RouteName = _routeElementController.RouteNameField.text;
            _selectedTransportRoute.TransportRouteElements = _routeElementController.TransportRouteElements;
            _transportRouteManager.OnTransportRouteChange(_selectedTransportRoute);
        });
    }

    public new void Reset()
    {
        _selectedTransportRoute = null;
        _createButton.gameObject.SetActive(true);
        _applyButton.gameObject.SetActive(false);
        _vehicleToggle.isOn = true;
        RouteToggle.isOn = false;
        RouteToggle.interactable = false;
        _createButton.interactable = false;
        _vehicleChooser.Reset();
        _settingController.Reset();
        _routeElementController.Reset();
    }

    public override void OnShortCut()
    {
        SetVisible(true);
    }

    #endregion

    public void CheckIfReady()
    {
        if (!_vehicleChooser.SelectedVehicle) return;
        if (_routeElementController.RouteNameField.text.Equals("")) return;
        if (_routeElementController.TransportRouteElementViews.Count <= 1) return;
        _createButton.interactable = true;
    }

    public void LoadTransportRoute(TransportRoute transportRoute)
    {
        _selectedTransportRoute = transportRoute;
        _createButton.gameObject.SetActive(false);
        _applyButton.gameObject.SetActive(true);
        SetVisible(true);
        VehicleChooser.SelectedVehicle = _selectedTransportRoute.Vehicle;
        RouteElementController.LoadTransportRoute(_selectedTransportRoute);
    }

    public override void SetVisible(bool visible)
    {
        base.SetVisible(visible);
        if (visible)
        {
            _routeElementController.RouteNameField.text = "Route #" + TransportRoute.RouteIndex;
        }
    }
}

[Serializable]
public class RouteVehicleChooser
{
    #region Attributes

    private TransportRouteCreateController _routeCreateController;

    [SerializeField] private Text _titleText;
    private TransportVehicle _selectedVehicle;
    [SerializeField] private GameObject _visibleGameObject;

    [Header("Vehicle Choice")] [SerializeField]
    private VehicleOptionView _vehicleOptionViewPrefab;

    [SerializeField] private Transform _scrollViewTransform;
    [SerializeField] private ToggleGroup _vehicleChoiceToggleGroup;

    [Header("Vehicle Information")] [SerializeField]
    private Text _speedText;

    [SerializeField] private Text _strengthText;
    [SerializeField] private Text _capacityText;
    [SerializeField] private Text _unloadSpeedText;
    [SerializeField] private Text _costText;
    [SerializeField] private Text _dailyCostText;

    public TransportVehicle SelectedVehicle
    {
        get { return _selectedVehicle; }

        set
        {
            _selectedVehicle = value;
            _routeCreateController.RouteToggle.interactable = _selectedVehicle;
            UpdateUi(SelectedVehicle);
        }
    }

    public GameObject VisibleGameObject
    {
        get { return _visibleGameObject; }

        set { _visibleGameObject = value; }
    }

    public TransportRouteCreateController RouteCreateController
    {
        set
        {
            _routeCreateController = value;
            FillVehicleView();
        }
    }

    #endregion

    #region Standard Methods

    public void Reset()
    {
        VisibleGameObject.SetActive(true);

        SelectedVehicle = null;
        _titleText.text = "Vehicle Amount";
        _speedText.text = "-";
        _strengthText.text = "-";
        _capacityText.text = "-";
        _unloadSpeedText.text = "-";
        _costText.text = "-";
        _dailyCostText.text = "-";
    }

    #endregion

    private void OnVehicleSelectClick(Vehicle vehicle)
    {
        Debug.Log("Vehicle selected: " + vehicle.name);
        if (vehicle is TransportVehicle)
        {
            SelectedVehicle = (TransportVehicle) vehicle;
        }
    }

    private void FillVehicleView()
    {
        VehicleManager vehicleManager = Object.FindObjectOfType<VehicleManager>();
        foreach (Vehicle vehicle in vehicleManager.VehicleList)
        {
            VehicleOptionView vehicleOptionObject =
                GameObject.Instantiate(_vehicleOptionViewPrefab, _scrollViewTransform);
            vehicleOptionObject.Vehicle = vehicle;
            vehicleOptionObject.SelectToggle.onValueChanged.AddListener(delegate(bool isActive)
            {
                if (isActive)
                {
                    OnVehicleSelectClick(vehicleOptionObject.Vehicle);
                }
            });
            vehicleOptionObject.SelectToggle.group = _vehicleChoiceToggleGroup;
        }
    }

    private void UpdateUi(TransportVehicle vehicle)
    {
        if (!vehicle) return;
        _titleText.text = vehicle.name + " Amount";
        _speedText.text = vehicle.Mover.MaxSpeed.ToString();
        _strengthText.text = "-";
        _capacityText.text = vehicle.TotalCapacity.ToString();
        _unloadSpeedText.text = vehicle.UnloadSpeed.ToString();
        _costText.text = "-";
        _dailyCostText.text = "-";
    }
}

[Serializable]
public class RouteElementController
{
    #region Attributes

    [Header("General")] [SerializeField] private InputField _routeNameField;
    [SerializeField] private GameObject _visibleGameObject;

    [Header("Route Element UI")] [SerializeField]
    private Transform _routeElementScrollView;

    [SerializeField] private TransportRouteElementView _routeElementPrefab;
    [SerializeField] private UserInformationPopup _userInformationPopup;

    private TransportRouteCreateController _routeCreateController;
    private RouteVehicleChooser _routeVehicleChooser;
    private RouteSettingController _settingController;

    public InputField RouteNameField
    {
        get { return _routeNameField; }

        set { _routeNameField = value; }
    }

    public List<TransportRouteElementView> TransportRouteElementViews
    {
        get
        {
            if (SelectedRouteElement) _settingController.Save(SelectedRouteElement.RouteElement);
            List<TransportRouteElementView> routeElementViews = new List<TransportRouteElementView>();
            for (int i = 0; i < _routeElementScrollView.childCount; i++)
            {
                routeElementViews.Add(GetElementView(i));
            }

            return routeElementViews;
        }
    }

    public List<TransportRouteElement> TransportRouteElements
    {
        get
        {
            if (SelectedRouteElement) _settingController.Save(SelectedRouteElement.RouteElement);
            List<TransportRouteElement> routeElements = new List<TransportRouteElement>();
            for (int i = 0; i < _routeElementScrollView.childCount; i++)
            {
                routeElements.Add(GetElementView(i).RouteElement);
            }

            return routeElements;
        }
    }

    public TransportRouteElementView SelectedRouteElement { get; set; }

    public GameObject VisibleGameObject
    {
        get { return _visibleGameObject; }

        set { _visibleGameObject = value; }
    }

    #endregion

    #region Standard Methods

    public void Reset()
    {
        for (int i = 0; i < _routeElementScrollView.childCount; i++)
        {
            TransportRouteElementView element = GetElementView(i);
            GameObject.Destroy(element.gameObject);
        }

        RouteNameField.text = "";
        VisibleGameObject.SetActive(false);
    }

    #endregion

    public TransportRouteCreateController RouteCreateController
    {
        set
        {
            _routeCreateController = value;
            _routeVehicleChooser = value.VehicleChooser;
            _settingController = value.SettingController;
            _userInformationPopup = value.UserInformationPopup;
            _routeNameField.onEndEdit.AddListener(delegate { value.CheckIfReady(); });
        }
    }

    private TransportRouteElementView GetElementView(int index)
    {
        return _routeElementScrollView.GetChild(index).gameObject.GetComponent<TransportRouteElementView>();
    }

    public void OnTransportStationClick(PathFindingNode pathFindingNode)
    {
        if (!VisibleGameObject.activeSelf) return;
        if (_routeVehicleChooser.SelectedVehicle == null)
        {
            _userInformationPopup.InformationText = "Vehicle needs to be set first.";
            return;
        }

        if (_routeElementScrollView.childCount > 0 &&
            GetElementView(_routeElementScrollView.childCount - 1).FromNode == pathFindingNode)
        {
            _userInformationPopup.InformationText = "Route needs to have unique stations.";
            return;
        }

        AddNode(pathFindingNode);
        
        _routeCreateController.CheckIfReady();
    }

    private void AddNode(PathFindingNode pathFindingNode)
    {
        TransportRouteElementView elementView = GameObject.Instantiate(_routeElementPrefab, _routeElementScrollView);
        elementView.SelectButton.onClick.AddListener(delegate
        {
            if (SelectedRouteElement)
            {
                _settingController.Save(SelectedRouteElement.RouteElement);
            }

            SelectedRouteElement = elementView;
            _settingController.LoadRouteElementSettings(elementView.RouteElement);
            Debug.Log("TransportElement selected; Setting Amount: " + elementView.RouteElement.RouteSettings.Count);
        });
        elementView.FromNode = pathFindingNode;
        if (_routeElementScrollView.childCount <= 1) return;
        TransportRouteElementView routeElementViewFirst = GetElementView(0);
        TransportRouteElementView routeElementViewSecondLast = GetElementView(_routeElementScrollView.childCount - 2);
        TransportRouteElementView routeElementViewLast = GetElementView(_routeElementScrollView.childCount - 1);
        routeElementViewSecondLast.ToNode = routeElementViewLast.FromNode;
        routeElementViewLast.ToNode = routeElementViewFirst.FromNode;
        if (_routeElementScrollView.childCount == 1)
        {
            SelectedRouteElement = GetElementView(0);
            _settingController.LoadRouteElementSettings(SelectedRouteElement.RouteElement);
            
        }
        _settingController.VisibleGameObject.SetActive(true);
    }

    public void LoadTransportRoute(TransportRoute transportRoute)
    {
        foreach (TransportRouteElement transportRouteElement in transportRoute.TransportRouteElements)
        {
            AddNode(transportRouteElement.FromNode);
        }
    }

    public void RemoveTransportRouteElement(TransportRouteElementView routeElementView)
    {
        for (int i = 0; i < _routeElementScrollView.childCount; i++)
        {
            TransportRouteElementView element = GetElementView(i);
            if (element.Equals(routeElementView))
            {
                GameObject.Destroy(element.gameObject);
                break;
            }
        }

        for (int i = 0; i < _routeElementScrollView.childCount; i++)
        {
            TransportRouteElementView view = GetElementView(i);
            if (view.ToNode != routeElementView.FromNode) continue;
            view.ToNode = GetElementView((i + 1) % _routeElementScrollView.childCount).FromNode;
            break;
        }
    }
}

/// <summary>
/// This class controls the RouteSettings of TransportRouteCreation.
/// </summary>
[Serializable]
public class RouteSettingController
{
    #region Attributes

    private TransportRouteCreateController _routeCreateController;
    private ProductSelector _routeSettingProductSelector;

    [SerializeField] private GameObject _visibleGameObject;
    [Header("Settings")] [SerializeField] private Transform _unloadSettingScrollView;
    [SerializeField] private Transform _loadSettingScrollView;
    [SerializeField] private TransportRouteProductView _elementPrefab;
    [SerializeField] private Text _fromToText;
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private RectTransform _productSelectorAnchor;

    public GameObject VisibleGameObject
    {
        get { return _visibleGameObject; }
    }

    #endregion

    #region Standard Methods

    public void Reset()
    {
        ClearObjects();
        _routeSettingProductSelector.VisibleGameObject.SetActive(false);
        VisibleGameObject.SetActive(false);
        _routeSettingProductSelector.OnProductSelectAction = null;
    }

    public void OnShow()
    {
        if (!RouteCreateController
            .VehicleChooser
            .SelectedVehicle) return;
        _routeSettingProductSelector.OnProductSelectAction = ProductSelected;
        for (int i = 0;
            i < RouteCreateController
                .VehicleChooser
                .SelectedVehicle
                .TotalCapacity;
            i++)
        {
            AddSetting(_unloadSettingScrollView);
            AddSetting(_loadSettingScrollView);
        }
    }

    #endregion

    public TransportRouteCreateController RouteCreateController
    {
        get { return _routeCreateController; }
        set
        {
            _routeCreateController = value;
            _routeSettingProductSelector = Object.FindObjectOfType<ProductSelector>();
        }
    }

    public void Save(TransportRouteElement transportRouteElement)
    {
        Debug.Log("Saved Settings (Needs optimization)");
        List<TransportRouteSetting> settings = new List<TransportRouteSetting>();

        ExtractSettingInformation(settings, _loadSettingScrollView, true);
        ExtractSettingInformation(settings, _unloadSettingScrollView, false);

        foreach (TransportRouteSetting setting in settings)
        {
            Debug.Log(setting.ToString());
        }

        transportRouteElement.RouteSettings = settings;
    }

    private void ExtractSettingInformation(List<TransportRouteSetting> settings, Transform parentTransform, bool isLoad)
    {
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            TransportRouteProductView routeProductView =
                parentTransform.GetChild(i).gameObject.GetComponent<TransportRouteProductView>();
            if (routeProductView.Product == null) continue;

            TransportRouteSetting setting = new TransportRouteSetting
                {IsLoad = isLoad, Amount = 1, ProductData = routeProductView.Product};
            settings.Add(setting);
        }
    }

    #region List Actions

    private TransportRouteProductView AddSetting(Transform parentTransform)
    {
        TransportRouteProductView elementGameObject = GameObject.Instantiate(_elementPrefab, parentTransform);
        elementGameObject.SelectionButton.group = _toggleGroup;
        //RouteCreateController.RouteElementController.SelectedRouteElement.TransportRouteElement.RouteSettings.Add(elementGameObject.Setting);
        elementGameObject.SelectionButton.onValueChanged.AddListener(delegate(bool value)
        {
            if (value)
            {
                _routeSettingProductSelector.VisibleGameObject.SetActive(!_routeSettingProductSelector.VisibleGameObject
                    .activeSelf);
                if (_routeCreateController.VisibleObject.activeSelf)
                {
                    _routeSettingProductSelector.VisibleGameObject.transform.position = _productSelectorAnchor.position;
                }
            }
            Debug.Log("Show ProductSelector");
            
            //RemoveRouteSetting(elementGameObject);
        });
        return elementGameObject;
    }

    public void RemoveRouteSetting(TransportRouteProductView routeSettingView)
    {
        Debug.Log("Remove Product");
        routeSettingView.Product = null;
        //	UnloadSettingScrollView.RemoveObject((RectTransform)routeSettingView.transform);
    }

    private void ClearObjects()
    {
        for (int i = 0; i < _unloadSettingScrollView.childCount; i++)
        {
            TransportRouteProductView routeProductView =
                _unloadSettingScrollView.GetChild(i).gameObject.GetComponent<TransportRouteProductView>();
            GameObject.Destroy(routeProductView.gameObject);
        }

        for (int i = 0; i < _loadSettingScrollView.childCount; i++)
        {
            TransportRouteProductView routeProductView =
                _loadSettingScrollView.GetChild(i).gameObject.GetComponent<TransportRouteProductView>();
            GameObject.Destroy(routeProductView.gameObject);
        }
    }

    #endregion

    private void ProductSelected(ProductData productData)
    {
        Debug.Log("Product " + productData.ProductName);
        foreach (Toggle activeToggle in _toggleGroup.ActiveToggles())
        {
            activeToggle.gameObject.GetComponent<TransportRouteProductView>().Product = productData;
        }
    }

    public void LoadRouteElementSettings(TransportRouteElement transportRouteElement)
    {
        //ClearObjects();
        string fromText = transportRouteElement.FromNode ? transportRouteElement.FromNode.BuildingName : "None";
        string toText = transportRouteElement.ToNode ? transportRouteElement.ToNode.BuildingName : "None";
        _fromToText.text = "From: " + fromText + "\nTo:" + toText;

        for (int i = 0; i < _unloadSettingScrollView.childCount; i++)
        {
            TransportRouteProductView routeProductView =
                _unloadSettingScrollView.GetChild(i).gameObject.GetComponent<TransportRouteProductView>();
            if (!routeProductView) continue;
            routeProductView.Product = null;
        }

        for (int i = 0; i < _loadSettingScrollView.childCount; i++)
        {
            TransportRouteProductView routeProductView =
                _loadSettingScrollView.GetChild(i).gameObject.GetComponent<TransportRouteProductView>();
            if (!routeProductView) continue;
            routeProductView.Product = null;
        }

        int unloadIndex = 0;
        int loadIndex = 0;

        for (int i = 0; i < transportRouteElement.RouteSettings.Count; i++)
        {
            TransportRouteSetting setting = transportRouteElement.RouteSettings[i];
            if (setting.IsLoad)
            {
                TransportRouteProductView transportRouteProductView = _loadSettingScrollView.GetChild(loadIndex).gameObject
                    .GetComponent<TransportRouteProductView>();
                transportRouteProductView.Setting = setting;
                loadIndex++;
            }
            else
            {
                TransportRouteProductView transportRouteProductView = _unloadSettingScrollView.GetChild(unloadIndex).gameObject
                    .GetComponent<TransportRouteProductView>();
                transportRouteProductView.Setting = setting;
                unloadIndex++;
            }
        }
    }
}