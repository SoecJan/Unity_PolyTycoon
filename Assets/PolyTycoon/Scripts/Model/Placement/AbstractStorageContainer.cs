using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A StorageContainer can take any kind of <see cref="ProductData"/> and store it.
/// The stored products can be picked up by <see cref="TransportVehicle"/>.
/// </summary>
public abstract class AbstractStorageContainer : PathFindingTarget, IProductEmitter, IProductReceiver
{
    private static ProductManager _productManager; // Used to return all products as possible received products.
    private Dictionary<ProductData, ProductStorage> _storedProducts; // Dictionary containg all stored products.

    protected override void Initialize()
    {
        base.Initialize();
        if (_productManager == null)
        {
            _productManager = FindObjectOfType<ProductManager>();
        }
        _storedProducts = new Dictionary<ProductData, ProductStorage>();
    }

    #region IProductEmitter

    public ProductStorage EmitterStorage(ProductData productData = null)
    {
        if (_storedProducts.Count == 0) return null;
        if (productData == null && _storedProducts.Count == 1) return _storedProducts.Values.ToArray()[0];
        return productData != null ? _storedProducts[productData] : null;
    }

    public List<ProductData> EmittedProductList()
    {
        return new List<ProductData>(_storedProducts.Keys);
    }
    
    #endregion

    #region IProductReceiver
    
    public ProductStorage ReceiverStorage(ProductData productData = null)
    {
        if (productData == null && _storedProducts.Count == 1) return _storedProducts.Values.ToArray()[0];
        if (productData == null) return null;
        if (!_storedProducts.ContainsKey(productData))
        {
            _storedProducts.Add(productData, new ProductStorage(productData, 99));
        }
        return _storedProducts[productData];
    }

    public List<ProductData> ReceivedProductList()
    {
        return _productManager.Products;
    }
    
    #endregion
}