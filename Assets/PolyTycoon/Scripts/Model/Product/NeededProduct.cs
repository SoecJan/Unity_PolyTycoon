using System;
using UnityEngine;

/// <summary>
/// This struct can be used to specify a needed product
/// </summary>
[Serializable]
public struct NeededProduct
{
    #region Attributes
    [SerializeField] private ProductData product;
    [SerializeField] private int amount;
    #endregion

    #region Getter & Setter
    public ProductData Product {
        get {
            return product;
        }
    }

    public int Amount {
        get {
            return amount;
        }
    }
    #endregion

    public override string ToString()
    {
        return Product.ProductName + ", Needed: " + Amount.ToString();
    }
}