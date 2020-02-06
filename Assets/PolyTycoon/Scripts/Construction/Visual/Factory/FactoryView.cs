using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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
            if (value == _factory && VisibleObject.activeSelf) return;
            _factory = value;
            if (!_factory) return;

            _titleText.text = _factory.name;
            LoadNeededProducts();
            SetVisible(true);
            StartCoroutine(UpdateUI());
        }
    }

    #endregion

    #region Methods

    private void Start()
    {
        _defaultProductSprite = _productImage.sprite;
        _exitButton.onClick.AddListener(delegate { SetVisible(false); });
    }

    private void LoadNeededProducts()
    {
        _factoryNeededProductView.ClearObjects();
        if (!_factory) return;
        _factoryNeededProductView.SetVisible(_factory.ReceiverStorage() != null || _factory.ReceivedProductList().Count > 0);

        // Add NeededProduct views to UI
        foreach (ProductData neededProducts in _factory.ReceivedProductList())
        {
            NeededProductView neededProductView = GameObject.Instantiate(
                _factoryNeededProductView.NeededProductViewPrefab,
                _factoryNeededProductView.ScrollView);
            neededProductView.ProductData = neededProducts;
            neededProductView.Text(_factory.ReceiverStorage(neededProducts));
            foreach (NeededProduct neededProduct in _factory.EmitterStorage().StoredProductData.NeededProduct)
            {
                if (neededProductView.ProductData.Equals(neededProduct.Product))
                {
                    neededProductView.StoredProductAmountText = neededProduct.Amount.ToString();
                }
            }
        }
    }

    private IEnumerator UpdateUI()
    {
        ProductStorage emitterStorage = _factory.EmitterStorage();
        Dictionary<ProductData, AmountProductView> _amountProductViewDict =
            new Dictionary<ProductData, AmountProductView>();
        for (int i = 0; i < _factoryNeededProductView.ScrollView.childCount; i++)
        {
            AmountProductView productView = _factoryNeededProductView.ScrollView.GetChild(i).gameObject
                .GetComponent<AmountProductView>();

            ProductStorage receiverStorage = _factory.ReceiverStorage(productView.ProductData);
            if (receiverStorage == null) continue;
            productView.Text(receiverStorage);
        }

        _amountLabel.text = emitterStorage.Amount + "/" + emitterStorage.MaxAmount;
        _productImage.sprite = emitterStorage.StoredProductData.ProductSprite;

        while (_factory && VisibleObject.activeSelf)
        {
//            _productionTimeSlider.value = _factory.ProductionProgress;
            yield return null;
        }

        _amountLabel.text = "Factory View";
        _productImage.sprite = _defaultProductSprite;
        _productionTimeSlider.value = 0;
        yield return null;
    }
    
    public void Reset()
    {
        Factory = null;
        _titleText.text = "Information";
        _amountLabel.text = "Select ProductData";
        _productImage.sprite = _defaultProductSprite;
        _productionTimeSlider.value = 0;

        Debug.Log("Reset FactoryView");
    }
}

#endregion

[Serializable]
struct FactoryProductToolTip
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
        get => _productNameText.text;
        set => _productNameText.text = value;
    }

    public string ProductInformationText
    {
        get => _productInformationText.text;
        set => _productInformationText.text = value;
    }
}

[Serializable]
struct FactoryNeededProductView
{
    [SerializeField] private GameObject _visibleGameObject;
    [SerializeField] private RectTransform _scrollView;

    [FormerlySerializedAs("amountProductStorageViewPrefab")]
    [FormerlySerializedAs("_neededProductStorageViewPrefab")]
    [SerializeField]
    private NeededProductView neededProductViewPrefab;

    public FactoryNeededProductView(GameObject visibleGameObject, RectTransform scrollViewTransform)
    {
        this._visibleGameObject = visibleGameObject;
        this._scrollView = scrollViewTransform;
        this.neededProductViewPrefab = Resources.Load<NeededProductView>(PathUtil.Get("NeededProductView"));
    }

    public RectTransform ScrollView => _scrollView;

    public NeededProductView NeededProductViewPrefab => neededProductViewPrefab;

    public void SetVisible(bool visible)
    {
        this._visibleGameObject.SetActive(visible);
    }

    public void ClearObjects()
    {
        for (int i = 0; i < _scrollView.childCount; i++)
        {
            Object.Destroy(_scrollView.transform.GetChild(i).gameObject);
        }
    }
}