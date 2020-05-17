using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Factory produces products out of source products.
/// <see cref="TransportVehicle"/> can drive to a Factory and unload/load products.
/// </summary>
public class ProductProcessorBehaviour : MonoBehaviour, IProductEmitter, IProductReceiver
{
    private ProductProcessorController _productProcessorController;
    private Coroutine _productionCoroutine;

    private void Start()
    {
        
    }

    private void OnDestroy()
    {
        StopCoroutine(_productionCoroutine);
    }

    public BuildingData BuildingData
    {
        set
        {
            if (!(value is BuildingProducerData producerData)) throw new ArgumentException("No BuildingProducerData");
            
            _productProcessorController = new ProductProcessorController(producerData.ProducedProduct, 10);
            _productionCoroutine = StartCoroutine(_productProcessorController.Produce());
        }
    }

    public ProductStorage EmitterStorage(ProductData productData = null)
    {
        return _productProcessorController.EmitterStorage(productData);
    }

    public List<ProductData> EmittedProductList()
    {
        return _productProcessorController.EmittedProductList();
    }

    public ProductStorage ReceiverStorage(ProductData productData = null)
    {
        return _productProcessorController.ReceiverStorage(productData);
    }

    public List<ProductData> ReceivedProductList()
    {
        return _productProcessorController.ReceivedProductList();
    }
}