using System.Collections.Generic;

public abstract class AbstractStorageContainer : PathFindingTarget, IProductEmitter, IProductReceiver
{
    private static ProductManager _productManager;
    private Dictionary<ProductData, ProductStorage> _storedProducts;
    
    protected override void Initialize()
    {
        base.Initialize();
        if (_productManager == null)
        {
            _productManager = FindObjectOfType<ProductManager>();
        }
        _storedProducts = new Dictionary<ProductData, ProductStorage>();
    }

    public Dictionary<ProductData, ProductStorage> StoredProducts()
    {
        return _storedProducts;
    }

    public ProductStorage EmitterStorage(ProductData productData = null)
    {
        if (_storedProducts.Count == 0) return null;
        if (productData == null && _storedProducts.Count == 1) return _storedProducts.Values.GetEnumerator().Current;
        return productData != null ? _storedProducts[productData] : null;
    }

    public bool IsEmitting(ProductData productData)
    {
        return _storedProducts.ContainsKey(productData);
    }

    public List<ProductData> EmittedProductList()
    {
        return new List<ProductData> (_storedProducts.Keys);
    }

    public ProductStorage ReceiverStorage(ProductData productData = null)
    {
        if (productData == null && _storedProducts.Count == 1) return _storedProducts.Values.GetEnumerator().Current;
        if (productData == null) return null;
        if (_storedProducts.ContainsKey(productData))
        {
            return _storedProducts[productData];
        }
        else
        {
            _storedProducts.Add(productData, new ProductStorage(productData));
            return _storedProducts[productData];
        }
    }

    public List<ProductData> ReceivedProductList()
    {
        return _productManager.Products;
    }
}