using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains the definition for a main building inside the city.
/// </summary>
public class CityMainBuilding : PathFindingTarget, IProductReceiver, IProductEmitter, ICityBuilding
{
    #region Attributes

    [SerializeField] private CityPlaceable _cityPlaceable;

    #endregion

    #region Getter & Setter

    public ProductStorage ReceiverStorage(ProductData productData = null)
    {
        return ((IProductReceiver) _cityPlaceable).ReceiverStorage(productData);
    }

    public List<ProductData> ReceivedProductList()
    {
        return ((IProductReceiver) _cityPlaceable).ReceivedProductList();
    }

    public int CurrentResidentCount { get; set; } = 1;

    public ProductStorage EmitterStorage(ProductData productData = null)
    {
        return ((IProductEmitter) _cityPlaceable).EmitterStorage(productData);
    }

    public List<ProductData> EmittedProductList()
    {
        return ((IProductEmitter) _cityPlaceable).EmittedProductList();
    }

    #endregion

    #region Methods

    void Start()
    {
        
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        RotateUsedCoords(transform.eulerAngles.y);
        if (!_cityPlaceable && transform.parent)
            _cityPlaceable = transform.parent.gameObject.GetComponent<CityPlaceable>();
    }

    public CityPlaceable CityPlaceable()
    {
        return _cityPlaceable;
    }

    #endregion
}