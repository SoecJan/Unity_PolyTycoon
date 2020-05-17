using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// This class controls the RouteSettings of TransportRouteCreation.
/// </summary>
[Serializable]
public class RouteCreationSettingsManager
{
    #region Attributes

    private ProductSelectionView _routeSettingProductSelectionView;
    private List<TransportRouteProductView> _productViews;
    private TransportRouteProductView _selectedProductView;

    [SerializeField] private GameObject _routeSettingVisibleGameObject;
    [SerializeField] private Transform _unloadSettingScrollView;
    [SerializeField] private Transform _loadSettingScrollView;
    [SerializeField] private TransportRouteProductView _routeProductElementPrefab;
    [FormerlySerializedAs("_fromToText")] 
    [SerializeField] private TextMeshProUGUI _loadAtText;
    [SerializeField] private TextMeshProUGUI _unloadAtText;
    [SerializeField] private ToggleGroup _routeSettingToggleGroup;

    public GameObject RouteSettingVisibleGameObject => _routeSettingVisibleGameObject;

    public ProductSelectionView RouteSettingProductSelectionView => _routeSettingProductSelectionView;

    private TransportRouteProductView SelectedProductView
    {
        get => _selectedProductView;
        set
        {
            _selectedProductView = value;
            if (_selectedProductView)
                _selectedProductView.SelectionToggle.isOn = true;
        }
    }
    #endregion

    #region Standard Methods

    public void Initialize()
    {
        _routeSettingProductSelectionView = Object.FindObjectOfType<ProductSelectionView>();
        _productViews = new List<TransportRouteProductView>();
    }

    public void Reset()
    {
        ClearObjects();
        _routeSettingProductSelectionView.VisibleGameObject.SetActive(false);
        RouteSettingVisibleGameObject.SetActive(false);
        _routeSettingProductSelectionView.OnProductSelectAction = null;
    }

    public void OnShow(int vehicleCapacity)
    {
        if (vehicleCapacity < 0) return;
        _routeSettingProductSelectionView.OnProductSelectAction = ProductSelected;
        for (int i = 0; i < vehicleCapacity; i++)
        {
            _productViews.Add(AddSetting(_loadSettingScrollView));
        }
        for (int i = 0; i < vehicleCapacity; i++)
        {
            _productViews.Add(AddSetting(_unloadSettingScrollView));
        }
    }

    #endregion

    public void Save(TransportRouteElement transportRouteElement)
    {
        Debug.Log("Saved Settings (Needs optimization)");
        List<TransportRouteSetting> settings = new List<TransportRouteSetting>();

        ExtractSettingInformation(settings, _loadSettingScrollView, true);
        ExtractSettingInformation(settings, _unloadSettingScrollView, false);

        transportRouteElement.RouteSettings = settings;
    }

    private void ExtractSettingInformation(List<TransportRouteSetting> settings, Transform parentTransform, bool isLoad)
    {
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            TransportRouteProductView routeProductView = parentTransform.GetChild(i).gameObject.GetComponent<TransportRouteProductView>();
            if (routeProductView.Product == null) continue;

            TransportRouteSetting setting = new TransportRouteSetting
                {IsLoad = isLoad, Amount = 1, ProductData = routeProductView.Product};
            settings.Add(setting);
        }
    }

    #region List Actions

    private TransportRouteProductView AddSetting(Transform parentTransform)
    {
        TransportRouteProductView transportRouteProductView =
            GameObject.Instantiate(_routeProductElementPrefab, parentTransform);
        transportRouteProductView.SelectionToggle.group = _routeSettingToggleGroup;
        transportRouteProductView.SelectionToggle.onValueChanged.AddListener(delegate (bool value)
        {
            if (value)
            {
                SelectedProductView = transportRouteProductView;
            }
        });
        return transportRouteProductView;
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
        
        _productViews.Clear();

        SelectedProductView = null;
    }

    #endregion

    private void ProductSelected(ProductData productData)
    {
        Debug.Log("Product " + productData.ProductName);
        SelectedProductView.Product = productData;
        for (int i = 0; i < _productViews.Count; i++)
        {
            TransportRouteProductView productView = _productViews[i];
            if (productView.Equals(SelectedProductView))
            {
                SelectedProductView = _productViews[(i + 1)%_productViews.Count];
                break;
            }
        }
    }

    public void LoadRouteElementSettings(TransportRouteElement transportRouteElement)
    {
        string fromText = transportRouteElement.FromNode ? transportRouteElement.FromNode.name : "None";
        string toText = transportRouteElement.ToNode ? transportRouteElement.ToNode.name : "None";
        _loadAtText.text = "Load at " + fromText;
        _unloadAtText.text = "Unload at " + toText;

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
                TransportRouteProductView transportRouteProductView = _loadSettingScrollView.GetChild(loadIndex)
                    .gameObject
                    .GetComponent<TransportRouteProductView>();
                transportRouteProductView.Setting = setting;
                loadIndex++;
            }
            else
            {
                TransportRouteProductView transportRouteProductView = _unloadSettingScrollView.GetChild(unloadIndex)
                    .gameObject
                    .GetComponent<TransportRouteProductView>();
                transportRouteProductView.Setting = setting;
                unloadIndex++;
            }
        }
        
        SelectedProductView = _productViews[0];
    }
}