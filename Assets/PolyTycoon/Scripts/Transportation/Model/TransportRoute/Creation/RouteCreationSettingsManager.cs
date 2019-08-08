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

    private ProductSelector _routeSettingProductSelector;
    private List<TransportRouteProductView> _productViews;
    private TransportRouteProductView _selectedProductView;

    [SerializeField] private GameObject _routeSettingVisibleGameObject;
    [SerializeField] private Transform _unloadSettingScrollView;
    [SerializeField] private Transform _loadSettingScrollView;
    [SerializeField] private TransportRouteProductView _routeProductElementPrefab;
    [FormerlySerializedAs("_fromToText")] [SerializeField] private TextMeshProUGUI _loadAtText;
    [SerializeField] private TextMeshProUGUI _unloadAtText;
    [SerializeField] private ToggleGroup _routeSettingToggleGroup;
    [SerializeField] private RectTransform _productSelectorAnchor;

    public GameObject RouteSettingVisibleGameObject => _routeSettingVisibleGameObject;

    #endregion

    #region Standard Methods

    public void Initialize()
    {
        _routeSettingProductSelector = Object.FindObjectOfType<ProductSelector>();
        _productViews = new List<TransportRouteProductView>();
    }

    public void Reset()
    {
        ClearObjects();
        _routeSettingProductSelector.VisibleGameObject.SetActive(false);
        RouteSettingVisibleGameObject.SetActive(false);
        _routeSettingProductSelector.OnProductSelectAction = null;
    }

    public void OnShow(TransportVehicleData selectedTransportVehicleData)
    {
        if (!selectedTransportVehicleData) return;
        _routeSettingProductSelector.OnProductSelectAction = ProductSelected;
        for (int i = 0; i < selectedTransportVehicleData.MaxCapacity; i++)
        {
            _productViews.Add(AddSetting(_loadSettingScrollView));
        }
        for (int i = 0; i < selectedTransportVehicleData.MaxCapacity; i++)
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
        TransportRouteProductView elementGameObject =
            GameObject.Instantiate(_routeProductElementPrefab, parentTransform);
        //RouteCreateController.RouteElementController.SelectedRouteElement.TransportRouteElement.RouteSettings.Add(elementGameObject.Setting);
        elementGameObject.SelectionButton.onClick.AddListener(delegate
        {
            ((RectTransform) _routeSettingProductSelector.VisibleGameObject.transform).anchoredPosition =
                _productSelectorAnchor.anchoredPosition;
            _routeSettingProductSelector.VisibleGameObject.SetActive(true);
            _selectedProductView = elementGameObject;
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
        _selectedProductView.Product = productData;
        for (int i = 0; i < _productViews.Count; i++)
        {
            TransportRouteProductView productView = _productViews[i];
            if (productView.Equals(_selectedProductView))
            {
                _selectedProductView = _productViews[(i + 1)%_productViews.Count];
                break;
            }
        }
    }

    public void LoadRouteElementSettings(TransportRouteElement transportRouteElement)
    {
        string fromText = transportRouteElement.FromNode ? transportRouteElement.FromNode.BuildingName : "None";
        string toText = transportRouteElement.ToNode ? transportRouteElement.ToNode.BuildingName : "None";
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
        
        _selectedProductView = _productViews[0];
    }
}