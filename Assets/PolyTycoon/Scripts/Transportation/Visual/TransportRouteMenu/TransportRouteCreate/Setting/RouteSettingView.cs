using System;
using UnityEngine;
using UnityEngine.UI;

public class RouteSettingView : PoolableObject
{
	private static RouteSettingView _selectedSettingView;
	private static RouteSettingProductSelector _productSelector;
	private static RouteVehicleChoiceController _vehicleChoiceController;
	private static RouteSettingController _routeSettingController;
	private TransportRouteSetting _routeSetting;
	private System.Action<RouteSettingView> _onValueChangeAction;

	[Header("Navigation")]
	[SerializeField] private Button _startEditButton;
	[SerializeField] private Button _endEditButton;
	[SerializeField] private Button _removeButton;

	[Header("ProductData")]
	[SerializeField] private Transform _productSelectorPositionTransform;
	[SerializeField] private Image _productImage;
	[SerializeField] private Button _productChangeButton;
	[SerializeField] private InputField _productAmountInputField;
	[SerializeField] private Slider _productAmountSlider;

	[Header("Loading")]
	[SerializeField] private Button _loadButton;
	[SerializeField] private Image _loadImage;
	[SerializeField] private Sprite _loadSprite;
	[SerializeField] private Sprite _unloadSprite;

	[Header("Placeholders")]
	[SerializeField] private Sprite _placeholder;

	public TransportRouteSetting RouteSetting {
		get {
			return _routeSetting;
		}

		set {
			_routeSetting = value;
			if (_routeSetting == null || !_routeSetting.ProductData) return;
			_loadImage.sprite = _routeSetting.IsLoad ? _loadSprite : _unloadSprite;
			_productImage.sprite = _routeSetting.ProductData.ProductSprite;
			_productAmountSlider.value = _routeSetting.Amount;
			_productAmountInputField.text = RouteSetting.Amount.ToString();
		}
	}

	public Action<RouteSettingView> OnValueChangeAction {
		get {
			return _onValueChangeAction;
		}

		set {
			_onValueChangeAction = value;
		}
	}

	private void Start()
	{
		//if (!_routeSettingController) _routeSettingController = FindObjectOfType<RouteSettingController>();
		//if (!_productSelector) _productSelector = FindObjectOfType<RouteSettingProductSelector>();
		//if (!_vehicleChoiceController) _vehicleChoiceController = FindObjectOfType<VehicleChoiceController>();

		_removeButton.onClick.AddListener(OnRemoveClick);
		_startEditButton.onClick.AddListener(OnStartEditClick);
		_endEditButton.onClick.AddListener(OnEndEditClick);
		_productChangeButton.onClick.AddListener(OnProductClick);
		_loadButton.onClick.AddListener(OnLoadClick);

		_productAmountSlider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
		_productAmountInputField.onEndEdit.AddListener(delegate { OnInputFieldEndEdit(); });

		_productAmountInputField.text = _productAmountSlider.minValue.ToString();
		OnStartEditClick();
	}

	private void OnStartEditClick()
	{
		if (_selectedSettingView)
		{
			_selectedSettingView.OnEndEditClick();
		}
		_selectedSettingView = this;
		_productSelector.OnProductSelectAction += OnProductChangeClick;
		_productSelector.VisibleGameObject.transform.position = _productSelectorPositionTransform.position;
		_removeButton.gameObject.SetActive(false);
		_startEditButton.gameObject.SetActive(false);
		_endEditButton.gameObject.SetActive(true);
		_loadButton.interactable = true;
		_productChangeButton.interactable = true;
		_productAmountInputField.interactable = true;
		_productAmountSlider.gameObject.SetActive(true);
		_productAmountSlider.maxValue = _vehicleChoiceController.SelectedVehicle.TotalCapacity;
	}

	private void OnEndEditClick()
	{
		_productSelector.OnProductSelectAction -= OnProductChangeClick;
		_productSelector.VisibleGameObject.SetActive(false);
		_removeButton.gameObject.SetActive(true);
		_startEditButton.gameObject.SetActive(true);
		_loadButton.interactable = false;
		_productChangeButton.interactable = false;
		_endEditButton.gameObject.SetActive(false);
		_productAmountInputField.interactable = false;
		_productAmountSlider.gameObject.SetActive(false);
		_selectedSettingView = null;
	}

	private void OnProductClick()
	{
		_productSelector.VisibleGameObject.SetActive(!_productSelector.VisibleGameObject.activeSelf);
	}

	private void OnProductChangeClick(ProductData productData)
	{
		RouteSetting.ProductData = productData;
		_productImage.sprite = productData.ProductSprite;
		_productSelector.VisibleGameObject.SetActive(false);
		OnValueChangeAction(this);
	}

	private void OnLoadClick()
	{
		if (RouteSetting == null) return;
		RouteSetting.IsLoad = !RouteSetting.IsLoad;
		_loadImage.sprite = RouteSetting.IsLoad ? _loadSprite : _unloadSprite;
		OnValueChangeAction(this);
	}

	private void OnSliderValueChange()
	{
		int amount = Mathf.RoundToInt(_productAmountSlider.value);

		if (RouteSetting == null || _productAmountInputField.text.Equals(amount.ToString())) return;

		_productAmountInputField.text = amount.ToString();
		RouteSetting.Amount = amount;
		OnValueChangeAction(this);
	}

	private void OnInputFieldEndEdit()
	{
		if (RouteSetting == null) return;
		try
		{
			int amount = int.Parse(_productAmountInputField.text);
			if (amount == Mathf.RoundToInt(_productAmountSlider.value)) return;
			if (amount < _productAmountSlider.minValue) _productAmountInputField.text = Mathf.RoundToInt(_productAmountSlider.minValue).ToString();
			if (amount > _productAmountSlider.maxValue) _productAmountInputField.text = Mathf.RoundToInt(_productAmountSlider.maxValue).ToString();

			_productAmountSlider.value = amount;
			RouteSetting.Amount = amount;
			OnValueChangeAction(this);
		}
		catch (FormatException)
		{
			_productAmountInputField.text = Mathf.RoundToInt(_productAmountSlider.value).ToString();
		}
	}

	private void OnRemoveClick()
	{
		//_routeSettingController.RemoveRouteSetting(this);
	}

	private void Reset()
	{
		RouteSetting = null;
		_productImage.sprite = _placeholder;
		_loadImage.sprite = _unloadSprite;
		_productAmountSlider.value = _productAmountSlider.minValue;
		_productAmountInputField.text = _productAmountSlider.minValue.ToString();
	}

	public override void Show()
	{
		OnStartEditClick();
		base.Show();
	}

	public override void Hide()
	{
		base.Hide();
		Reset();
	}
}
