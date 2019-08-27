using System;
using System.Collections;
using TMPro;
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

    private Factory _factory;

    [Header("General")] [SerializeField] private Button _exitButton;

    [Header("Factory Information")] [SerializeField]
    private TextMeshProUGUI _titleText;

    [SerializeField] private TextMeshProUGUI _amountLabel;
    [SerializeField] private Slider _productionTimeSlider;
    [SerializeField] private Image _productImage;
    private Sprite _defaultProductSprite;

    [Header("ToolTip")] [SerializeField] private FactoryProductToolTip _factoryProductToolTip;

    [Header("Needed Product")] [SerializeField]
    private FactoryNeededProductView _factoryNeededProductView;

    #endregion

    #region Getter & Setter

    public Factory Factory
    {
        set
        {
            if (value == _factory) return;
            if (_factory) _factory.Outline.enabled = false;
            _factory = value;
            if (!_factory) return;

            _factory.Outline.enabled = true;
            OnProductChange(_factory.ProductData);
//            LoadNeededProducts();
            _titleText.text = _factory.BuildingName;
            SetVisible(true);
            StartCoroutine(UpdateUI());
        }
    }

    #endregion

    #region Methods

    private void Start()
    {
        _defaultProductSprite = _productImage.sprite;
//        _transportRouteCreateController = FindObjectOfType<TransportRouteCreateController>();
//        _productSelector = FindObjectOfType<ProductSelector>();
//        _routeCreateButton.onClick.AddListener(delegate { _transportRouteCreateController.SetVisible(true); });
        _exitButton.onClick.AddListener(delegate { SetVisible(false); });
//        _productChangeButton.onClick.AddListener(OnProductChangeClick);
    }

    private void LoadNeededProducts()
    {
        _factoryNeededProductView.ClearObjects();
        if (!_factory) return;
        _factoryNeededProductView.VisibleGameObject.SetActive(_factory.ReceiverStorage() != null || _factory.ReceivedProductList().Count > 0);
        
        // Add NeededProduct views to UI
        foreach (ProductData neededProducts in _factory.ReceivedProductList())
        {
            NeededProductView neededProductView = GameObject.Instantiate(
                _factoryNeededProductView.NeededProductViewPrefab,
                _factoryNeededProductView.ScrollView);
            neededProductView.ProductData = neededProducts;
            neededProductView.Text(_factory.ReceiverStorage(neededProducts));
            foreach (NeededProduct neededProduct in _factory.ProductData.NeededProduct)
            {
                if (neededProductView.ProductData.Equals(neededProduct.Product))
                {
                    neededProductView.StoredProductAmountText = neededProduct.Amount.ToString();
                }
            }
        }
    }

//    private void OnProductChangeClick()
//    {
//        _productSelector.VisibleGameObject.transform.position = _productSelectorPosition.position;
//        _productSelector.VisibleGameObject.SetActive(!_productSelector.VisibleGameObject.activeSelf);
//    }

    private void OnProductChange(ProductData productData)
    {
        if (!_factory) return;
        _factory.ProductData = productData;
//        _productSelector.VisibleGameObject.SetActive(false);
        LoadNeededProducts();

        if (!productData) return;
        // Update Tooltip
        _factoryProductToolTip.Image.sprite = productData.ProductSprite;
        _factoryProductToolTip.ProductNameText = productData.ProductName;
        _factoryProductToolTip.ProductInformationText =
            productData.Description + "\nProduction Time: " + productData.ProductionTime;
    }

    private IEnumerator UpdateUI()
    {
        while (_factory && VisibleObject.activeSelf)
        {
            if (_factory.ProductData != null)
            {
                _amountLabel.text = _factory.EmitterStorage().Amount.ToString() + "/" +
                                    _factory.EmitterStorage().MaxAmount.ToString();
                _productionTimeSlider.value = _factory.ProductionProgress;
                _productImage.sprite = _factory.ProductData.ProductSprite;
                for (int i = 0; i < _factoryNeededProductView.ScrollView.childCount; i++)
                {
                    AmountProductView productView = _factoryNeededProductView.ScrollView.GetChild(i).gameObject
                        .GetComponent<AmountProductView>();
                    if (((IProductReceiver) _factory).ReceiverStorage(productView.ProductData) == null) continue;
                    ProductStorage productStorage =
                        ((IProductReceiver) _factory).ReceiverStorage(productView.ProductData);
                    productView.Text(productStorage);
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
//        _productSelector.VisibleGameObject.SetActive(false);
        Debug.Log("Reset FactoryView");
    }

    #endregion

    [Serializable]
    private struct FactoryProductToolTip
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _productNameText;
        [SerializeField] private TextMeshProUGUI _productInformationText;

        public FactoryProductToolTip(Image image, TextMeshProUGUI productNameText,
            TextMeshProUGUI productInformationText)
        {
            _image = image;
            _productNameText = productNameText;
            _productInformationText = productInformationText;
        }

        public Image Image => _image;

        public string ProductNameText
        {
            get { return _productNameText.text; }
            set { _productNameText.text = value; }
        }

        public string ProductInformationText
        {
            get { return _productInformationText.text; }
            set { _productInformationText.text = value; }
        }
    }

    [Serializable]
    private struct FactoryNeededProductView
    {
        [SerializeField] private GameObject _visibleGameObject;
        [SerializeField] private RectTransform _scrollView;

        [FormerlySerializedAs("amountProductStorageViewPrefab")]
        [FormerlySerializedAs("_neededProductStorageViewPrefab")]
        [SerializeField]
        private NeededProductView neededProductViewPrefab;

        public RectTransform ScrollView => _scrollView;

        public NeededProductView NeededProductViewPrefab => neededProductViewPrefab;

        public GameObject VisibleGameObject => _visibleGameObject;

        public void ClearObjects()
        {
            for (int i = 0; i < _scrollView.childCount; i++)
            {
                Destroy(_scrollView.transform.GetChild(i).gameObject);
            }
        }
    }
}