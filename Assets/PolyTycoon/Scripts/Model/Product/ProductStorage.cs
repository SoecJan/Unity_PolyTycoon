using System;
using UnityEngine;


/// <summary>
/// This is the base class of Products that need to be stored.
/// </summary>
[System.Serializable]
public class ProductStorage
{
    #region Attributes

    private System.Action<ProductStorage, int> _onAmountChange;
    [SerializeField] private ProductData _storedProductData;
    [SerializeField] private int _maxAmount;
    [SerializeField] private int _storedAmount;

    #endregion

    #region Constructors & Getter & Setter

    public ProductStorage()
    {
    }

    public ProductStorage(ProductData productData)
    {
        _storedProductData = productData;
    }

    public ProductStorage(ProductData productData, int maxAmount) : this(productData)
    {
        _maxAmount = maxAmount;
    }

    public ProductStorage(ProductData productData, int maxAmount, int storedAmount) : this(productData, maxAmount)
    {
        _storedAmount = storedAmount;
    }

    public ProductData StoredProductData
    {
        get => _storedProductData;

        set => _storedProductData = value;
    }

    public int MaxAmount
    {
        get => _maxAmount;

        set => _maxAmount = value;
    }

    public int Amount
    {
        get => _storedAmount;
    }

    public Action<ProductStorage, int> OnAmountChange
    {
        get => _onAmountChange;
        set => _onAmountChange = value;
    }

    public void SetAmount(int amount)
    {
        if (amount > this._maxAmount) throw new OverflowException();
        int difference = amount - this._storedAmount;
        this._storedAmount = amount;
        OnAmountChange?.Invoke(this, difference);
    }
    
    public void Add(int amount)
    {
        if (this._storedAmount + amount > this._maxAmount) throw new OverflowException();
        this._storedAmount += amount;
        OnAmountChange?.Invoke(this, amount);
    }

    #endregion

    #region Methods

    public override string ToString()
    {
        return (StoredProductData == null ? "" : StoredProductData.ToString()) + "; Amount: " + Amount;
    }

    /// <summary>
    /// Returns a new instance of the used object.
    /// </summary>
    /// <returns>New Instance</returns>
    public ProductStorage Clone()
    {
        ProductStorage storage = new ProductStorage
        {
            StoredProductData = this._storedProductData,
            MaxAmount = this._maxAmount,
        };
        storage.Add(this._storedAmount);
        return storage;
    }

    public override bool Equals(object obj)
    {
        return obj is ProductStorage storage &&
               storage.StoredProductData.ProductName.Equals(this.StoredProductData.ProductName);
    }

    public override int GetHashCode()
    {
        var hashCode = -929180017;
        hashCode = hashCode * -1521134295 + _maxAmount.GetHashCode();
        hashCode = hashCode * -1521134295 + _storedAmount.GetHashCode();
        hashCode = hashCode * -1521134295 + MaxAmount.GetHashCode();
        hashCode = hashCode * -1521134295 + Amount.GetHashCode();
        return hashCode;
    }

    #endregion
}