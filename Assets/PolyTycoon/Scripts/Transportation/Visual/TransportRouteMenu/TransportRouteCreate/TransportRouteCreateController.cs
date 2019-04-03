using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class TransportRouteCreateController : AbstractUi
{
	[Header("Navigation")]
	[SerializeField] private Button _createRouteButton;
	[SerializeField] private Button _showButton;
	[SerializeField] private Button _exitButton;
	[SerializeField] private Toggle _routeToggle;
	[SerializeField] private Button _createToggle;
	[Header("Controller")]
	[SerializeField] private TransportRouteManager _transportRouteManager;
	[SerializeField] private RouteVehicleChoiceController _vehicleChoiceController;
	[SerializeField] private RouteSettingController _settingController;
	[SerializeField] private RouteElementController _routeElementController;

	public RouteSettingController SettingController {
		get {
			return _settingController;
		}

		set {
			_settingController = value;
		}
	}

	public RouteElementController RouteElementController {
		get {
			return _routeElementController;
		}

		set {
			_routeElementController = value;
		}
	}

	public RouteVehicleChoiceController VehicleChoiceController {
		get {
			return _vehicleChoiceController;
		}

		set {
			_vehicleChoiceController = value;
		}
	}

	public Toggle RouteToggle {
		get { return _routeToggle; }
		set { _routeToggle = value; }
	}

	private void Start()
	{
		_createRouteButton.onClick.AddListener(_transportRouteManager.OnTransportRouteCreate);
		_showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf); });
		_exitButton.onClick.AddListener(delegate { SetVisible(false); Reset(); });
		_vehicleChoiceController.RouteCreateController = this;
		_settingController.RouteCreateController = this;
		_routeElementController.RouteCreateController = this;
		RouteToggle.onValueChanged.AddListener(delegate
		{
			VehicleChoiceController.VisibleGameObject.SetActive(false);
			RouteElementController.VisibleGameObject.SetActive(true);
		});
		_routeToggle.onValueChanged.AddListener(delegate (bool value)
		{
			_vehicleChoiceController.VisibleGameObject.SetActive(!value);
			_routeElementController.VisibleGameObject.SetActive(value);
			if (value) SettingController.OnShow();
		});
		_createToggle.onClick.AddListener(delegate
		{
			Debug.Log("Go!");
			_transportRouteManager.OnTransportRouteCreate();
		});
	}

	public void CheckIfReady()
	{
		if (!_vehicleChoiceController.SelectedVehicle) return;
		if (_routeElementController.TransportRouteElementViews.Count <= 1) return;
		_createToggle.interactable = true;
	}

	public void LoadTransportRoute(TransportRoute transportRoute)
	{
		SetVisible(true);
		VehicleChoiceController.SelectedVehicle = transportRoute.Vehicle;
		RouteElementController.LoadTransportRoute(transportRoute);
	}

	public new void Reset()
	{
		_vehicleChoiceController.Reset();
		_settingController.Reset();
		_routeElementController.Reset();
	}

	public override void OnShortCut()
	{
		SetVisible(true);
	}
}

[Serializable]
public class RouteVehicleChoiceController
{
	[SerializeField] private Text _titleText;
	private TransportVehicle _selectedVehicle;
	[SerializeField] private GameObject _visibleGameObject;

	[Header("Vehicle Choice")]
	[SerializeField] private VehicleOptionView _vehicleOptionViewPrefab;
	[SerializeField] private Transform _scrollViewTransform;
	[SerializeField] private ToggleGroup _vehicleChoiceToggleGroup;

	[Header("Vehicle Information")]
	[SerializeField] private Text _speedText;
	[SerializeField] private Text _strengthText;
	[SerializeField] private Text _capacityText;
	[SerializeField] private Text _unloadSpeedText;
	[SerializeField] private Text _costText;
	[SerializeField] private Text _dailyCostText;

	public TransportVehicle SelectedVehicle {
		get {
			return _selectedVehicle;
		}

		set {
			_selectedVehicle = value;
			RouteToggle.interactable = _selectedVehicle;
			UpdateUi(SelectedVehicle);
		}
	}

	public GameObject VisibleGameObject {
		get {
			return _visibleGameObject;
		}

		set {
			_visibleGameObject = value;
		}
	}

	public TransportRouteCreateController RouteCreateController {
		set {
			RouteToggle = value.RouteToggle;
			FillVehicleView();
		}
	}

	public Toggle RouteToggle { get; set; }

	private void OnVehicleSelectClick(Vehicle vehicle)
	{
		Debug.Log("Vehicle selected: " + vehicle.name);
		if (vehicle is TransportVehicle)
			SelectedVehicle = (TransportVehicle)vehicle;
	}

	private void FillVehicleView()
	{
		VehicleManager vehicleManager = Object.FindObjectOfType<VehicleManager>();
		foreach (Vehicle vehicle in vehicleManager.VehicleList)
		{
			VehicleOptionView vehicleOptionObject = GameObject.Instantiate(_vehicleOptionViewPrefab, _scrollViewTransform);
			vehicleOptionObject.Vehicle = vehicle;
			vehicleOptionObject.SelectToggle.onValueChanged.AddListener(delegate (bool isActive)
			{
				if (isActive)
					OnVehicleSelectClick(vehicleOptionObject.Vehicle);
			});
			vehicleOptionObject.SelectToggle.group = _vehicleChoiceToggleGroup;
		}
	}

	public void Reset()
	{
		_vehicleChoiceToggleGroup.SetAllTogglesOff();
		SelectedVehicle = null;
		_titleText.text = "Vehicle Amount";
		_speedText.text = "Speed";
		_strengthText.text = "Strength";
		_capacityText.text = "Capacity";
		_unloadSpeedText.text = "Unload";
		_costText.text = "Cost";
		_dailyCostText.text = "DailyCost";
	}

	private void UpdateUi(TransportVehicle vehicle)
	{
		if (!vehicle) return;
		_titleText.text = vehicle.name + " Amount";
		_speedText.text = vehicle.Mover.MaxSpeed.ToString();
		_strengthText.text = "1";
		_capacityText.text = vehicle.TotalCapacity.ToString();
		_unloadSpeedText.text = vehicle.UnloadSpeed.ToString();
		_costText.text = "100k";
		_dailyCostText.text = "2k";

	}
}

[Serializable]
public class RouteElementController
{
	[Header("General")]
	[SerializeField] private InputField _routeNameField;
	[SerializeField] private GameObject _visibleGameObject;

	[Header("Route Element UI")]
	[SerializeField] private Transform _routeElementScrollView;
	[SerializeField] private TransportRouteElementView _routeElementPrefab;
	[SerializeField] private UserInformationPopup _userInformationPopup;

	private TransportRouteCreateController _routeCreateController;
	private RouteVehicleChoiceController _routeVehicleChoiceController;
	private RouteSettingController _settingController;

	public TransportRouteCreateController RouteCreateController {
		set
		{
			_routeCreateController = value;
			_routeVehicleChoiceController = value.VehicleChoiceController;
			_settingController = value.SettingController;
			_userInformationPopup = Object.FindObjectOfType<UserInformationPopup>();
			_routeNameField.onEndEdit.AddListener(delegate { value.CheckIfReady(); });
		}
	}

	public InputField RouteNameField {
		get {
			return _routeNameField;
		}

		set {
			_routeNameField = value;
		}
	}

	public List<TransportRouteElementView> TransportRouteElementViews {
		get {
			if (SelectedRouteElement)_settingController.Save(SelectedRouteElement.RouteElement);
			List<TransportRouteElementView> routeElementViews = new List<TransportRouteElementView>();
			for (int i = 0; i < _routeElementScrollView.childCount; i++)
			{
				routeElementViews.Add(GetElementView(i));
			}
			return routeElementViews;
		}
	}

	public TransportRouteElementView SelectedRouteElement { get; set; }

	public GameObject VisibleGameObject {
		get {
			return _visibleGameObject;
		}

		set {
			_visibleGameObject = value;
		}
	}

	private TransportRouteElementView GetElementView(int index)
	{
		return _routeElementScrollView.GetChild(index).gameObject.GetComponent<TransportRouteElementView>();
	}

	public void OnTransportStationClick(PathFindingNode pathFindingNode)
	{
		if (!VisibleGameObject.activeSelf) return;
		if (_routeVehicleChoiceController.SelectedVehicle == null)
		{
			_userInformationPopup.InformationText = "Vehicle needs to be set first.";
			return;
		}
		TransportRouteElementView elementView = GameObject.Instantiate(_routeElementPrefab, _routeElementScrollView);
		elementView.SelectButton.onClick.AddListener(delegate
		{
			_settingController.Save(elementView.RouteElement);
			SelectedRouteElement = elementView;
			_settingController.LoadRouteElementSettings(elementView.RouteElement);
			Debug.Log("TransportElement selected");
		});
		elementView.FromNode = pathFindingNode;
		if (_routeElementScrollView.childCount > 1)
		{
			TransportRouteElementView routeElementViewFirst = GetElementView(0);
			TransportRouteElementView routeElementViewSecondLast = GetElementView(_routeElementScrollView.childCount - 2);
			TransportRouteElementView routeElementViewLast = GetElementView(_routeElementScrollView.childCount - 1);
			routeElementViewSecondLast.ToNode = routeElementViewLast.FromNode;
			routeElementViewLast.ToNode = routeElementViewFirst.FromNode;
			_settingController.VisibleGameObject.SetActive(true);
			//_routeCreateController.CheckIfReady();
		}
	}

	public void LoadTransportRoute(TransportRoute transportRoute)
	{
		//foreach (TransportRouteElement transportRouteElement in transportRoute.TransportRouteElements)
		//{
		//	TransportRouteElementView elementView = GameObject.Instantiate(_routeElementPrefab, _routeElementScrollView);
		//	elementView.TransportRouteElement = transportRouteElement;
		//}
	}

	public void RemoveTransportRouteElement(RouteElementView routeElementView)
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

	public void Reset()
	{
		for (int i = 0; i < _routeElementScrollView.childCount; i++)
		{
			TransportRouteElementView element = GetElementView(i);
			GameObject.Destroy(element.gameObject);
		}
		RouteNameField.text = "";
		_settingController.VisibleGameObject.SetActive(false);
	}
}

/// <summary>
/// This class controls the RouteSettings of TransportRouteCreation.
/// </summary>
[Serializable]
public class RouteSettingController
{
	private TransportRouteCreateController _routeCreateController;
	private RouteSettingProductSelector _routeSettingProductSelector;

	[SerializeField] private GameObject _visibleGameObject;
	[Header("Settings")]
	[SerializeField] private Transform _unloadSettingScrollView;
	[SerializeField] private Transform _loadSettingScrollView;
	[SerializeField] private TransportProductView _elementPrefab;
	[SerializeField] private ToggleGroup _toggleGroup;

	public TransportRouteCreateController RouteCreateController {
		get { return _routeCreateController; }
		set {
			_routeCreateController = value;
			_routeSettingProductSelector = Object.FindObjectOfType<RouteSettingProductSelector>();
		}
	}

	public GameObject VisibleGameObject {
		get { return _visibleGameObject; }
	}

	public void OnShow()
	{
		_routeSettingProductSelector.OnProductSelectAction += ProductSelected;
		for (int i = 0; i < _routeCreateController
							.VehicleChoiceController
							.SelectedVehicle
							.TotalCapacity; i++)
		{
			AddSetting(_unloadSettingScrollView);
			AddSetting(_loadSettingScrollView);
		}
	}

	public void Save(TransportRouteElement transportRouteElement)
	{
		Debug.Log("Saved Settings (Needs optimization)");
		List<TransportRouteSetting> settings = new List<TransportRouteSetting>();

		for (int i = 0; i < _unloadSettingScrollView.childCount; i++)
		{
			TransportProductView productView = _unloadSettingScrollView.GetChild(i).gameObject.GetComponent<TransportProductView>();
			if (productView.Product == null) continue;

			TransportRouteSetting setting = new TransportRouteSetting();
			setting.IsLoad = false;
			setting.Amount = 1;
			setting.ProductData = productView.Product;
			settings.Add(setting);
		}

		for (int i = 0; i < _unloadSettingScrollView.childCount; i++)
		{
			TransportProductView productView = _unloadSettingScrollView.GetChild(i).gameObject.GetComponent<TransportProductView>();
			if (productView.Product == null) continue;

			TransportRouteSetting setting = new TransportRouteSetting();
			setting.IsLoad = true;
			setting.Amount = 1;
			setting.ProductData = productView.Product;
			settings.Add(setting);
		}

		foreach (TransportRouteSetting setting in settings)
		{
			Debug.Log(setting.ToString());
		}
		transportRouteElement.RouteSettings = settings;
	}

	private TransportProductView AddSetting(Transform parentTransform)
	{
		TransportProductView elementGameObject = GameObject.Instantiate(_elementPrefab, parentTransform);
		//RouteCreateController.RouteElementController.SelectedRouteElement.TransportRouteElement.RouteSettings.Add(elementGameObject.Setting);
		elementGameObject.SelectionButton.onClick.AddListener(delegate
		{
			_routeSettingProductSelector.VisibleGameObject.SetActive(!_routeSettingProductSelector.VisibleGameObject.activeSelf);
			Debug.Log("Show ProductSelector");

			//RemoveRouteSetting(elementGameObject);
		});
		return elementGameObject;
	}

	private void ProductSelected(ProductData productData)
	{
		Debug.Log("Product " + productData.ProductName);

		for (int i = 0; i < _unloadSettingScrollView.childCount; i++)
		{
			TransportProductView productView = _unloadSettingScrollView.GetChild(i).gameObject.GetComponent<TransportProductView>();
			if (!productView.Product)
			{
				productView.Product = productData;
				return;
			}
		}

		for (int i = 0; i < _loadSettingScrollView.childCount; i++)
		{
			TransportProductView productView = _loadSettingScrollView.GetChild(i).gameObject.GetComponent<TransportProductView>();
			if (!productView.Product)
			{
				productView.Product = productData;
				return;
			}
		}

	}

	public void RemoveRouteSetting(TransportProductView routeSettingView)
	{
		Debug.Log("Remove Product");
		routeSettingView.Product = null;
		//	UnloadSettingScrollView.RemoveObject((RectTransform)routeSettingView.transform);
	}

	public void Reset()
	{
		ClearObjects();
		_routeSettingProductSelector.VisibleGameObject.SetActive(false);
	}

	public void LoadRouteElementSettings(TransportRouteElement transportRouteElement)
	{
		ClearObjects();

		foreach (TransportRouteSetting routeSetting in transportRouteElement.RouteSettings)
		{
			if (routeSetting.IsLoad)
			{
				TransportProductView instantiatedGameObject = AddSetting(_loadSettingScrollView);
				instantiatedGameObject.Setting = routeSetting;
			}
			else
			{
				TransportProductView instantiatedGameObject = AddSetting(_unloadSettingScrollView);
				instantiatedGameObject.Setting = routeSetting;
			}
		}
	}

	private void ClearObjects()
	{
		for (int i = 0; i < _unloadSettingScrollView.childCount; i++)
		{
			TransportProductView productView = _unloadSettingScrollView.GetChild(i).gameObject.GetComponent<TransportProductView>();
			GameObject.Destroy(productView.gameObject);
		}
		for (int i = 0; i < _loadSettingScrollView.childCount; i++)
		{
			TransportProductView productView = _unloadSettingScrollView.GetChild(i).gameObject.GetComponent<TransportProductView>();
			GameObject.Destroy(productView.gameObject);
		}
	}
}
