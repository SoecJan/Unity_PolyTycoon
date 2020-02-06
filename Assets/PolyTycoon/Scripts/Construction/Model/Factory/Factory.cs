using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

/**
 * Business Logic of a factory
 */
public class FactoryController : IProductEmitter, IProductReceiver
{
    private Dictionary<ProductData, ProductStorage> _neededProducts; // Dict of needed Products
    private ProductStorage _producedProduct; // The currently produced product

    public FactoryController([NotNull] ProductData producedProduct, int maxAmount)
    {
        _neededProducts = new Dictionary<ProductData, ProductStorage>();
        foreach (NeededProduct neededProduct in producedProduct.NeededProduct)
        {
            _neededProducts.Add(neededProduct.Product, new ProductStorage(neededProduct.Product, maxAmount));
        }
        
        _producedProduct = new ProductStorage(producedProduct, maxAmount);
    }

    public ProductStorage EmitterStorage(ProductData productData = null)
    {
        if (productData == null) return _producedProduct;
        return _producedProduct.StoredProductData.Equals(productData) ? _producedProduct : null;
    }

    public List<ProductData> EmittedProductList()
    {
        return new List<ProductData> {_producedProduct.StoredProductData};
    }
    
    public ProductStorage ReceiverStorage(ProductData productData = null)
    {
        if (_neededProducts.Count == 0) return null;
        if (productData == null && _neededProducts.Count == 1) return _neededProducts[_neededProducts.Keys.ToArray()[0]];
        return productData != null && _neededProducts.ContainsKey(productData) ? _neededProducts[productData] : null;
    }

    public List<ProductData> ReceivedProductList()
    {
        return new List<ProductData> (_neededProducts.Keys);
    }

    /// <summary>
    /// Handles the Production process. Needs to be stopped if the game object is deleted.
    /// </summary>
    public IEnumerator Produce()
    {
        while (true)
        {
            yield return new WaitUntil(IsProductionReady);
            foreach (NeededProduct neededProduct in _producedProduct.StoredProductData.NeededProduct)
            {
                ReceiverStorage(neededProduct.Product).Add(-neededProduct.Amount);
            }
            yield return new WaitForSeconds(_producedProduct.StoredProductData.ProductionTime);
            EmitterStorage().Add(1);
        }
        // ReSharper disable once IteratorNeverReturns
    }
    
    /// <summary>
    /// Checks if another product can be produced.
    /// Checks Storage Capacity and the presence of needed products
    /// </summary>
    /// <returns>true if a new product can be produced</returns>
    private bool IsProductionReady()
    {
        return EmitterStorage().Amount < EmitterStorage().MaxAmount && HasEnoughNeededProducts();
    }

    private bool HasEnoughNeededProducts()
    {
        foreach (NeededProduct neededProduct in _producedProduct.StoredProductData.NeededProduct)
        {
            if (neededProduct.Amount > ReceiverStorage(neededProduct.Product).Amount)
            {
                return false;
            }
        }
        return true;
    }
}

/// <summary>
/// A Factory produces products out of source products.
/// <see cref="TransportVehicle"/> can drive to a Factory and unload/load products.
/// </summary>
public class Factory : MonoBehaviour, IProductEmitter, IProductReceiver
{
    private FactoryController _factoryController;
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
            if (!(value is BuildingProducerData producerData)) throw new ArgumentException();
            
            _factoryController = new FactoryController(producerData.ProducedProduct, 10);
            _productionCoroutine = StartCoroutine(_factoryController.Produce());
        }
    }

    public ProductStorage EmitterStorage(ProductData productData = null)
    {
        return _factoryController.EmitterStorage(productData);
    }

    public List<ProductData> EmittedProductList()
    {
        return _factoryController.EmittedProductList();
    }

    public ProductStorage ReceiverStorage(ProductData productData = null)
    {
        return _factoryController.ReceiverStorage(productData);
    }

    public List<ProductData> ReceivedProductList()
    {
        return _factoryController.ReceivedProductList();
    }
}