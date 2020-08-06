using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains the definition for a main building inside the city.
/// </summary>
public class CityMainBuilding : PathFindingTarget, IProductReceiver, IProductEmitter, ICityBuilding
{
    #region Attributes

    #endregion

    #region Getter & Setter

    public ProductStorage ReceiverStorage(ProductData productData = null)
    {
        return ((IProductReceiver) CityPlaceable).ReceiverStorage(productData);
    }

    public List<ProductData> ReceivedProductList()
    {
        return ((IProductReceiver) CityPlaceable).ReceivedProductList();
    }

    public CityPlaceable CityPlaceable { get; set; }
    public int CurrentResidentCount { get; set; } = 1;

    public ProductStorage EmitterStorage(ProductData productData = null)
    {
        return ((IProductEmitter) CityPlaceable).EmitterStorage(productData);
    }

    public List<ProductData> EmittedProductList()
    {
        return ((IProductEmitter) CityPlaceable).EmittedProductList();
    }

    #endregion

    #region Methods

    public override void Start()
    {
        base.Start();
        // RotateUsedCoords(transform.eulerAngles.y);
        if (!CityPlaceable && transform.parent)
            CityPlaceable = transform.parent.gameObject.GetComponent<CityPlaceable>();
    }

    #endregion
}