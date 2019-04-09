using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// This class provides a UI that displays <see cref="Factory"/> data to the Player.
/// The Player can set what productData this factory is supposed to produce.
/// </summary>
public class FactoryView : AbstractUi
{
    #region Attributes

    private static TransportRouteCreateController _transportRouteCreateController;
    private static ProductSelector _productSelector;
    private Factory _factory;

    [Header("General")] 
    [SerializeField] private Button _exitButton;
    [SerializeField] private Transform _productSelectorPosition;

    [Header("Factory Information")] 
    [SerializeField] private Text _titleText;
    [SerializeField] private Text _amountLabel;

    [SerializeField] private Slider _productionTimeSlider;
    [SerializeField] private Image _productImage;
    private Sprite _defaultProductSprite;
    [SerializeField] private Button _productChangeButton;
    [Header("ToolTip")] 
    [SerializeField] private FactoryProductToolTip _factoryProductToolTip;

    [Header("Needed Product")] 
    [SerializeField] private FactoryNeededProductView _factoryNeededProductView;

    [Header("Transport Route")] 
    [SerializeField] private Button _routeCreateButton;

    #endregion

    #region Getter & Setter

    public Factory Factory
    {
        set
        {
            _factory = value;
            if (!_factory)
            {
                _productSelector.OnProductSelectAction = null;
                return;
            }

            LoadNeededProducts();
            _titleText.text = _factory.BuildingName;
            SetVisible(true);
            _productSelector.OnProductSelectAction = OnProductChange;
            _productSelector.gameObject.SetActive(true);
            StartCoroutine(UpdateUI());
        }
    }

    #endregion

    #region Methods

    private void Start()
    {
        _defaultProductSprite = _productImage.sprite;
        _transportRouteCreateController = FindObjectOfType<TransportRouteCreateController>();
        _productSelector = FindObjectOfType<ProductSelector>();
        _routeCreateButton.onClick.AddListener(delegate { _transportRouteCreateController.SetVisible(true); });
        _exitButton.onClick.AddListener(delegate { SetVisible(false); });
        _productChangeButton.onClick.AddListener(OnProductChangeClick);
    }

    private void LoadNeededProducts()
    {
        _factoryNeededProductView.ClearObjects();
        if (!_factory) return;
        _factoryNeededProductView.VisibleGameObject.SetActive(_factory.NeededProducts() != null);
        if (_factory.NeededProducts() == null)
        {
            return;
        }

        foreach (ProductStorage neededProductStorage in _factory.NeededProducts().Values)
        {
            NeededProductStorageView neededProductStorageView = GameObject.Instantiate(_factoryNeededProductView.NeededProductStorageViewPrefab,
                _factoryNeededProductView.ScrollView);
            neededProductStorageView.ProductData = neededProductStorage.StoredProductData;
            neededProductStorageView.NeededAmountText.text = neededProductStorage.Amount + "/" + neededProductStorage.MaxAmount;
            neededProductStorageView.StoredProductAmountText.text = _factory.ProductData.NeededProduct.Amount.ToString();
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
        _factoryProductToolTip.ProductInformationText.text =
            productData.Description + "\nProduction Time: " + productData.ProductionTime;

        LoadNeededProducts();
    }

    private IEnumerator UpdateUI()
    {
        while (_factory && VisibleObject.activeSelf)
        {
            if (_factory.ProductData != null)
            {
                _amountLabel.text = _factory.ProducedProductStorage().Amount.ToString() + "/" +
                                    _factory.ProducedProductStorage().MaxAmount.ToString();
                _productionTimeSlider.value = _factory.ProductionProgress;
                _productImage.sprite = _factory.ProductData.ProductSprite;
                for (int i = 0; i < _factoryNeededProductView.ScrollView.childCount; i++)
                {
                    NeededProductView productView = _factoryNeededProductView.ScrollView.GetChild(i).gameObject.GetComponent<NeededProductView>();
                    if (!((IConsumer) _factory).NeededProducts().ContainsKey(productView.ProductData)) continue;
                    ProductStorage productStorage = ((IConsumer) _factory).NeededProducts()[productView.ProductData];
                    productView.NeededAmountText.text = productStorage.Amount + "/" + productStorage.MaxAmount;
                }
            }
            else
            {
                _amountLabel.text = "Select ProductData";
                _productImage.sprite = _defaultProductSprite;
                _productionTimeSlider.value = 0;
            }

            yield return 1;
        }
    }

    public override void Reset()
    {
        Factory = null;
        _titleText.text = "Information";
        _amountLabel.text = "Select ProductData";
        _productImage.sprite = _defaultProductSprite;
        _productionTimeSlider.value = 0;
        _productSelector.VisibleGameObject.SetActive(false);
        Debug.Log("Reset FactoryView");
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

        public Image Image
        {
            get { return _image; }

            set { _image = value; }
        }

        public Text ProductNameText
        {
            get { return _productNameText; }

            set { _productNameText = value; }
        }

        public Text ProductInformationText
        {
            get { return _productInformationText; }

            set { _productInformationText = value; }
        }
    }

    [Serializable]
    private struct FactoryNeededProductView
    {
        [SerializeField] private GameObject _visibleGameObject;
        [SerializeField] private RectTransform _scrollView;
        [SerializeField] private NeededProductStorageView _neededProductStorageViewPrefab;

        public RectTransform ScrollView
        {
            get { return _scrollView; }

            set { _scrollView = value; }
        }

        public NeededProductStorageView NeededProductStorageViewPrefab
        {
            get { return _neededProductStorageViewPrefab; }

            set { _neededProductStorageViewPrefab = value; }
        }

        public GameObject VisibleGameObject
        {
            get { return _visibleGameObject; }

            set { _visibleGameObject = value; }
        }

        public void ClearObjects()
        {
            for (int i = 0; i < _scrollView.childCount; i++)
            {
                Destroy(_scrollView.transform.GetChild(i).gameObject);
            }
        }
    }
}