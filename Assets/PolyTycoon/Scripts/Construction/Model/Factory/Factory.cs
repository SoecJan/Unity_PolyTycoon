﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public interface IFactory : IProductEmitter, IProductReceiver
{
    ProductData ProductData { get; set; }
    float ProductionProgress { get; }
    bool IsProductSelectable { get; }
}

public class Factory : PathFindingTarget, IFactory
{
    #region Attributes

    [SerializeField] private Dictionary<ProductData, ProductStorage> _neededProducts; // Dict of needed Products
    [SerializeField] private ProductStorage _producedProduct; // The currently produced product
    private bool _isProductSelectable = true;
    private bool _isProducing;
    private float _elapsedTime; // Time elapsed since the begin of the production process
    private int _tempMaxAmount = 20;

    private Coroutine _produceCoroutine;
//	private Dictionary<BiomeGenerator.Biome, float> _biomeValueDictionary;

    #endregion

    #region Getter & Setter

    public ProductData ProductData
    {
        get => EmitterStorage()?.StoredProductData;

        set
        {
            // Don't alter if this is the same Product
            if (EmitterStorage()?.StoredProductData != null && EmitterStorage().StoredProductData.Equals(value)) return;

            // Change Product to selected
            EmitterStorage().StoredProductData = value;
            if (ProductData) InitializeProduction();
        }
    }

    public float ProductionProgress => _elapsedTime / ProductData.ProductionTime;

    public bool IsProductSelectable => _isProductSelectable;

    //public Dictionary<BiomeGenerator.Biome, float> BiomeValueDictionary {
    //	get {
    //		return _biomeValueDictionary;
    //	}

    //	set {
    //		_biomeValueDictionary = value;
    //		Debug.Log("Biome Values Set");
    //		foreach (float biomeValue in _biomeValueDictionary.Values)
    //		{
    //			Debug.Log(biomeValue);
    //		}
    //	}
    //}

    public ProductStorage ReceiverStorage(ProductData productData = null)
    {
        if (_neededProducts.Count == 0) return null;
        if (productData == null && _neededProducts.Count == 1) return _neededProducts[_neededProducts.Keys.ToArray()[0]];
        return productData != null ? _neededProducts[productData] : null;
    }

    public List<ProductData> ReceivedProductList()
    {
        return new List<ProductData> (_neededProducts.Keys);
    }

    #endregion

    #region Default Methods

    protected override void Initialize()
    {
        base.Initialize();
        _neededProducts = new Dictionary<ProductData, ProductStorage>();
        // Initialize production if a product has been set in advance.
        if (_producedProduct.StoredProductData == null) return;

        InitializeProduction();
        _isProductSelectable = false;
    }

    private void InitializeProduction()
    {
        EmitterStorage().MaxAmount = _tempMaxAmount;
        EmitterStorage().Amount = 0;
        _isProducing = false;
        _elapsedTime = 0f;

        // Setup needed Products
        _neededProducts.Clear();
        if (ProductData.NeededProduct.Length > 0)
        {
            foreach (NeededProduct neededProduct in ProductData.NeededProduct)
            {
                _neededProducts.Add(neededProduct.Product, new ProductStorage(neededProduct.Product, _tempMaxAmount));
            }
        }

        // Start production of the new product
        if (_produceCoroutine != null)
        {
            StopCoroutine(_produceCoroutine);
            _produceCoroutine = null;
        }

        _produceCoroutine = StartCoroutine(Produce());
    }

    private void Update()
    {
        // Added to show production progress to the player
        if (_isProducing)
        {
            _elapsedTime += Time.deltaTime;
        }
    }

    #endregion

    #region Production

    /// <summary>
    /// Handles the Production process. Handled by <see cref="set_ProductData"/>.
    /// </summary>
    IEnumerator Produce()
    {
        while (ProductData != null)
        {
            // Wait until there are enough needed products
            while (!IsProductionReady())
            {
                yield return new WaitForSeconds(0.5f);
            }

            // Remove needed products from storage
            foreach (NeededProduct neededProduct in ProductData.NeededProduct)
            {
                ReceiverStorage(neededProduct.Product).Amount -= neededProduct.Amount;
            }

            // Produce
            _isProducing = true;
            yield return new WaitForSeconds(ProductData.ProductionTime);
            EmitterStorage().Amount += 1;
            _isProducing = false;
            _elapsedTime = 0f;

            // Debug
            Debug.Log("Product Finished: " + EmitterStorage().StoredProductData.ProductName + ": " +
                      EmitterStorage().Amount);
        }
    }

    private bool IsProductionReady()
    {
        bool productionReady = ProductData != null && EmitterStorage().Amount < EmitterStorage().MaxAmount;
        if (!productionReady) return false;
        foreach (NeededProduct neededProduct in ProductData.NeededProduct)
        {
            if (neededProduct.Amount > ReceiverStorage(neededProduct.Product).Amount)
            {
                productionReady = false;
            }
        }

        return productionReady;
    }

    #endregion

    public ProductStorage EmitterStorage(ProductData productData = null)
    {
        if (productData == null) return _producedProduct;
        return productData.Equals(_producedProduct.StoredProductData) ? _producedProduct : null;
    }

    public List<ProductData> EmittedProductList()
    {
        return new List<ProductData> {_producedProduct.StoredProductData};
    }
}