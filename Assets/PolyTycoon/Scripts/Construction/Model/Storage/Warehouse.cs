using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : PathFindingNode, IStore
{
    [SerializeField] private int _maxCapacity = 256;
    private Dictionary<ProductData, ProductStorage> _storedProducts;

    protected override void Initialize()
    {
        base.Initialize();
        IsClickable = true;
        _storedProducts = new Dictionary<ProductData, ProductStorage>();
    }

    public override bool IsTraversable()
    {
        return false;
    }

    public override bool IsNode()
    {
        return true;
    }

    public Dictionary<ProductData, ProductStorage> StoredProducts()
    {
        return _storedProducts;
    }
}
