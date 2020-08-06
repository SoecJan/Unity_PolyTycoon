using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouteElementActionView : MonoBehaviour
{
    private TransportRouteSetting _routeSetting;
    [SerializeField] private TMP_Text _waitConditionText;
    [SerializeField] private TMP_Text _loadConditionText;
    [SerializeField] private TMP_InputField _amountText;
    [SerializeField] private TMP_Text _productText;

    [SerializeField] private Button _waitConditionButton;
    [SerializeField] private Button _loadConditionButton;
    [SerializeField] private Button _productButton;
    [SerializeField] private Button _deleteButton;
    public System.Action<RouteElementActionView> _onDelete;
    
    public TransportRouteElement RouteElement { get; set; }
    
    public TransportRouteSetting RouteSetting
    {
        get => _routeSetting;
        set
        {
            _routeSetting = value;
            _amountText.text = _routeSetting.Amount.ToString();
            _productText.text = value.ProductData ? _routeSetting.ProductData.ProductName : "No Products";
            _loadConditionText.text = _routeSetting.IsLoad ? "Load" : "Unload";
            _waitConditionText.text = RouteSetting.WaitStatus.ToString();
        }
    }

    private void Start()
    {
        _deleteButton.onClick.AddListener(delegate { _onDelete?.Invoke(this); });
        _waitConditionButton.onClick.AddListener(delegate {
            switch (RouteSetting.WaitStatus)
            {
                case TransportRouteSetting.RouteSettingWaitStatus.WAITFOR:
                    RouteSetting.WaitStatus = TransportRouteSetting.RouteSettingWaitStatus.DONTWAIT;
                    break;
                case TransportRouteSetting.RouteSettingWaitStatus.DONTWAIT:
                    RouteSetting.WaitStatus = TransportRouteSetting.RouteSettingWaitStatus.WAITFOR;
                    break;
                default:
                    RouteSetting.WaitStatus = TransportRouteSetting.RouteSettingWaitStatus.DONTWAIT;
                    break;
            }
            _waitConditionText.text = RouteSetting.WaitStatus.ToString();
        });
        
        _loadConditionButton.onClick.AddListener(delegate
        {
            List<ProductData> shownProducts;
            bool previous = RouteSetting.IsLoad;
            shownProducts = !RouteSetting.IsLoad
                ? RouteElement.FromNode.GetComponent<IProductReceiver>().ReceivedProductList()
                : RouteElement.FromNode.GetComponent<IProductEmitter>().EmittedProductList() ;
            RouteSetting.IsLoad = shownProducts.Count > 0;
            if (previous == RouteSetting.IsLoad) return;
            _loadConditionText.text = RouteSetting.IsLoad ? "Load" : "Unload";
            SetupProductButton(shownProducts);
        });
        
        _amountText.onValueChanged.AddListener(delegate(string value)
        {
            int output;
            if (!int.TryParse(value, out output)) return;
            if (output <= 0) _amountText.text = "1";
            RouteSetting.Amount = output;
        });
        
        _productButton.onClick.AddListener(delegate
        {
            List<ProductData> shownProducts;
            shownProducts = RouteSetting.IsLoad 
                ? RouteElement.FromNode.GetComponent<IProductEmitter>().EmittedProductList() 
                : RouteElement.FromNode.GetComponent<IProductReceiver>().ReceivedProductList();

            SetupProductButton(shownProducts);
        });
    }

    private void SetupProductButton(List<ProductData> shownProducts)
    {
        for (int i = 0; i < shownProducts.Count; i++)
        {
            ProductData productData = shownProducts[i];
            if (!productData.ProductName.Equals(_productText.text)) continue;
                
            ProductData nextProduct = shownProducts[Util.Mod(i, shownProducts.Count)];
            _productText.text = nextProduct.ProductName;
            RouteSetting.ProductData = nextProduct;
            break;
        }
    }
}
