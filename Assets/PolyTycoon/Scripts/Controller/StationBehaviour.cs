using System;
using System.Collections.Generic;
using UnityEngine;

public class StationBehaviour : MonoBehaviour, IProductEmitter, IProductReceiver
{
    [SerializeField] private List<IProductEmitter> _emitters;
    [SerializeField] private List<IProductReceiver> _receivers;
    private StationRadiusBehaviour _stationRadiusBehaviour;
    public List<IProductEmitter> Emitters
    {
        get => _emitters;
    }

    public List<IProductReceiver> Receivers
    {
        get => _receivers;
    }

    // Start is called before the first frame update
    void Start()
    {
        _emitters = new List<IProductEmitter>();
        _receivers = new List<IProductReceiver>();
        _stationRadiusBehaviour = GetComponentInChildren<StationRadiusBehaviour>();
    }

    public ProductStorage EmitterStorage(ProductData productData = null)
    {
        ProductStorage productStorage = null;
        foreach (IProductEmitter productEmitter in _emitters)
        {
            productStorage = productEmitter.EmitterStorage(productData);
            if (productStorage != null && productStorage.Amount > 0) break;
        }
        return productStorage;
    }

    public List<ProductData> EmittedProductList()
    {
        List<ProductData> productDatas = new List<ProductData>();
        foreach (IProductEmitter productEmitter in _emitters)
        {
            productDatas.AddRange(productEmitter.EmittedProductList());
        }
        return productDatas;
    }

    public ProductStorage ReceiverStorage(ProductData productData = null)
    {
        ProductStorage productStorage = null;
        foreach (IProductReceiver productReceiver in _receivers)
        {
            productStorage = productReceiver.ReceiverStorage(productData);
            if (productStorage != null && productStorage.Amount > 0) break;
        }
        return productStorage;
    }

    public List<ProductData> ReceivedProductList()
    {
        List<ProductData> productDatas = new List<ProductData>();
        foreach (IProductReceiver productReceiver in _receivers)
        {
            productDatas.AddRange(productReceiver.ReceivedProductList());
        }
        return productDatas;
    }

    private void OnMouseEnter()
    {
        _stationRadiusBehaviour.VisibleObj.SetActive(true);
    }
    
    private void OnMouseExit()
    {
        _stationRadiusBehaviour.VisibleObj.SetActive(false);
    }
}
