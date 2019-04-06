using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class provides a UI that displays <see cref="Factory"/> data to the Player.
/// The Player can set what productData this factory is supposed to produce.
/// </summary>
public class FactoryView : AbstractUi
{
	#region Attributes

	private static TransportRouteCreateController _transportRouteCreateController;
	private static RouteSettingProductSelector _productSelector;
	private Factory _factory;

	[Header("General")]
	[SerializeField] private Button _exitButton;
	[SerializeField] private Transform _productSelectorPosition;
	[Header("Factory Information")]
	[SerializeField] private Text _amountLabel;
	[SerializeField] private Slider _productionTimeSlider;
	[SerializeField] private Image _productImage;
	[SerializeField] private Button _productChangeButton;
	[Header("ToolTip")]
	[SerializeField] private FactoryProductToolTip _factoryProductToolTip;
	[Header("Needed Product")]
	[SerializeField] private FactoryNeededProductView _factoryNeededProductView;

	[Header("Transport Route")]
	[SerializeField] private Button _routeCreateButton;
	#endregion

	#region Getter & Setter
	public Factory Factory {
		set {
			_factory = value;
			if (!_factory)
			{
				_productSelector.OnProductSelectAction -= OnProductChange;
				return;
			}
			LoadNeededProducts();
			SetVisible(true);
			_productSelector.OnProductSelectAction += OnProductChange;
			_productSelector.gameObject.SetActive(true);
			StartCoroutine(UpdateUI());
		}
	}
	#endregion

	#region Methods

	private void Start()
	{
		_transportRouteCreateController = FindObjectOfType<TransportRouteCreateController>();
		_productSelector = FindObjectOfType<RouteSettingProductSelector>();
		_routeCreateButton.onClick.AddListener(delegate { _transportRouteCreateController.SetVisible(true); });
		_exitButton.onClick.AddListener(delegate { SetVisible(false); });
		_productChangeButton.onClick.AddListener(OnProductChangeClick);
	}

	private void LoadNeededProducts()
	{
		_factoryNeededProductView.ScrollView.ClearObjects();
		if (!_factory) return;
		_factoryNeededProductView.VisibleGameObject.SetActive(_factory.NeededProducts() != null);
		if (_factory.NeededProducts() == null)
		{
			return;
		}
		foreach (ProductStorage neededProductStorage in _factory.NeededProducts().Values)
		{
			GameObject prefabInstance = _factoryNeededProductView.ScrollView.AddObject((RectTransform)_factoryNeededProductView.ProductUiSlotPrefab.transform);
			NeededProductStorageView productView = prefabInstance.GetComponent<NeededProductStorageView>();
			productView.ProductData = neededProductStorage.StoredProductData;
			productView.NeededAmountText.text = neededProductStorage.Amount + "/" + neededProductStorage.MaxAmount;
			productView.StoredProductAmountText.text = _factory.ProductData.NeededProduct.Amount.ToString();
		}
	}

	private void OnProductChangeClick()
	{
		_productSelector.VisibleGameObject.transform.position = _productSelectorPosition.position;
		_productSelector.VisibleGameObject.SetActive(!_productSelector.VisibleGameObject.activeSelf);
	}

	private void OnProductChange(ProductData productData)
	{
		if (!_factory) return;
		_factory.ProductData = productData;
		_productSelector.VisibleGameObject.SetActive(false);

		_factoryProductToolTip.Image.sprite = productData.ProductSprite;
		_factoryProductToolTip.ProductNameText.text = productData.ProductName;
		_factoryProductToolTip.ProductInformationText.text = productData.Description + "\nProduction Time: " + productData.ProductionTime;

		LoadNeededProducts();
	}

	private IEnumerator UpdateUI()
	{
		while (_factory)
		{
			if (_factory.ProductData != null)
			{
				_amountLabel.text = _factory.ProductStorage().Amount.ToString() + "/" + _factory.ProductStorage().MaxAmount.ToString();
				_productionTimeSlider.value = _factory.ProductionProgress;
				_productImage.sprite = _factory.ProductData.ProductSprite;
				foreach (RectTransform rectTransform in _factoryNeededProductView.ScrollView.ContentObjects)
				{
					NeededProductView productView = rectTransform.gameObject.GetComponent<NeededProductView>();
					ProductStorage productStorage = ((IConsumer)_factory).NeededProducts()[productView.ProductData];
					productView.NeededAmountText.text = productStorage.Amount + "/" + productStorage.MaxAmount;
				}
			}
			else
			{
				_amountLabel.text = "Select ProductData";
				_productImage.sprite = null;
				_productionTimeSlider.value = 0;
			}
			yield return 1;
		}
	}

	public new void Reset()
	{
		Factory = null;
		_amountLabel.text = "Select ProductData";
		_productImage.sprite = null;
		_productionTimeSlider.value = 0;
		_productSelector.VisibleGameObject.SetActive(false);
	}
	#endregion

	[Serializable]
	private struct FactoryProductToolTip
	{
		[SerializeField] private Image _image;
		[SerializeField] private Text _productNameText;
		[SerializeField] private Text _productInformationText;

		public FactoryProductToolTip(Image image, Text productNameText, Text productInformationText)
		{
			_image = image;
			_productNameText = productNameText;
			_productInformationText = productInformationText;
		}

		public Image Image {
			get {
				return _image;
			}

			set {
				_image = value;
			}
		}

		public Text ProductNameText {
			get {
				return _productNameText;
			}

			set {
				_productNameText = value;
			}
		}

		public Text ProductInformationText {
			get {
				return _productInformationText;
			}

			set {
				_productInformationText = value;
			}
		}
	}

	[Serializable]
	private struct FactoryNeededProductView
	{
		[SerializeField] private GameObject _visibleGameObject;
		[SerializeField] private ScrollViewHandle _scrollView;
		[SerializeField] private NeededProductStorageView _productUiSlotPrefab;

		public ScrollViewHandle ScrollView {
			get {
				return _scrollView;
			}

			set {
				_scrollView = value;
			}
		}

		public NeededProductStorageView ProductUiSlotPrefab {
			get {
				return _productUiSlotPrefab;
			}

			set {
				_productUiSlotPrefab = value;
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
	}
}