using System.Collections.Generic;

public abstract class AbstractStorageContainer : PathFindingNode, IStore
{
    private Dictionary<ProductData, ProductStorage> _storedProducts;
    protected override void Initialize()
    {
        _storedProducts = new Dictionary<ProductData, ProductStorage>();
    }

    public Dictionary<ProductData, ProductStorage> StoredProducts()
    {
        return _storedProducts;
    }
}