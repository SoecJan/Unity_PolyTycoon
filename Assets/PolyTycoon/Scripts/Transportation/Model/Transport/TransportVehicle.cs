using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ITransport
{
    int CurrentCapacity { get; }
    int TotalCapacity { get; set; }

    float TransferTime { get; set; }
    int TransferAmount { get; set; }

    ProductStorage TransportStorage(ProductData productData);
    List<ProductData> LoadedProducts { get; }

    IEnumerator Load(TransportRouteElement element);

    IEnumerator Unload(TransportRouteElement element);
}

[Serializable]
public class TransportVehicleController : ITransport
{
    private Dictionary<ProductData, ProductStorage> _transporterStorage; // The Products that are loaded
    public int CurrentCapacity { get; } = 0;
    public int TotalCapacity { get; set; } = 4;
    public float TransferTime { get; set; } = 1f;
    public int TransferAmount { get; set; } = 1;
    public List<ProductData> LoadedProducts => new List<ProductData>(_transporterStorage.Keys);

    public TransportVehicleController()
    {
        _transporterStorage = new Dictionary<ProductData, ProductStorage>();
    }

    public ProductStorage TransportStorage(ProductData productData)
    {
        return _transporterStorage[productData];
    }

    public IEnumerator Load(TransportRouteElement element)
    {
        if (element.FromNode is IProductEmitter producer)
        {
            foreach (TransportRouteSetting setting in element.RouteSettings)
            {
                bool isUnload = !setting.IsLoad;
                bool isEmitterProducingProduct =
                    setting.ProductData.Equals(producer.EmitterStorage().StoredProductData);

                if (!isUnload || !isEmitterProducingProduct) continue;

                if (!_transporterStorage.ContainsKey(setting.ProductData))
                {
                    _transporterStorage.Add(setting.ProductData, new ProductStorage(setting.ProductData)
                        {MaxAmount = TotalCapacity, Amount = 0});
                }

                ProductStorage emitterStorage = producer.EmitterStorage(setting.ProductData);
                ProductStorage truckStorage = _transporterStorage[setting.ProductData];
                int loadAmount = setting.Amount;
                while (loadAmount > 0 && truckStorage.Amount != truckStorage.MaxAmount)
                {
                    while (producer.EmitterStorage().Amount == 0)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }

                    loadAmount--;
                    emitterStorage.Amount -= TransferAmount;
                    truckStorage.Amount += TransferAmount;
                    yield return new WaitForSeconds(TransferTime);
                }
            }
        }
    }

    public IEnumerator Unload(TransportRouteElement element)
    {
        // Unload Products
        if (element == null || !(element.ToNode is IProductReceiver consumer)) yield break;

        foreach (TransportRouteSetting setting in element.RouteSettings)
        {
            bool isLoad = setting.IsLoad;
            bool isLoadedProductReceiver = consumer.ReceiverStorage(setting.ProductData) != null;
            bool hasProductLoaded = _transporterStorage.ContainsKey(setting.ProductData);
            // if: setting is for loading, receiver not in need of the product or the truck has none of the product
            if (isLoad || !isLoadedProductReceiver || !hasProductLoaded) continue;

            ProductStorage receiverStorage = consumer.ReceiverStorage(setting.ProductData);
            ProductStorage truckStorage = _transporterStorage[setting.ProductData];
            int unloadAmount = setting.Amount; // The amount that is supposed to be transferred

            // Handle actual unloading
            while (unloadAmount > 0 && truckStorage.Amount > 0 &&
                   receiverStorage.Amount < receiverStorage.MaxAmount)
            {
                unloadAmount--;
                truckStorage.Amount -= TransferAmount;
                receiverStorage.Amount += TransferAmount;
                yield return new WaitForSeconds(TransferTime);
            }
        }
    }
}

/// <summary>
/// Vehicle that can transport products from one place to the other.
/// </summary>
public class TransportVehicle : Vehicle
{
    #region Attributes

    [SerializeField] private TransportVehicleController _transportController;
    private TransportRoute _transportRoute; // Route this vehicle drives on

    #endregion

    #region Getter & Setter

    public int CurrentCapacity
    {
        get => _transportController.CurrentCapacity;
    }

    public int TotalCapacity
    {
        get => _transportController.TotalCapacity;
        set => _transportController.TotalCapacity = value;
    }

    public float TransferTime
    {
        get => _transportController.TransferTime;
        set => _transportController.TransferTime = value;
    }

    public ProductStorage TransportStorage(ProductData productData)
    {
        return _transportController.TransportStorage(productData);
    }

    public List<ProductData> LoadedProducts
    {
        get => _transportController.LoadedProducts;
    }

    public static Action<TransportVehicle> OnClickAction { get; set; }

    public TransportRoute TransportRoute
    {
        get => _transportRoute;

        set
        {
            _transportRoute = value;
            base.PathList = _transportRoute.PathList;
        }
    }

    #endregion

    #region Methods

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            OnClickAction(this);
        }
    }

    protected override void OnArrive()
    {
        StartCoroutine(HandleLoad());
    }

    private IEnumerator HandleLoad() // TODO: Missing: Maximum Amounts of Storage
    {
        // Unload Products
        int routeIndex = (RouteIndex) % _transportRoute.TransportRouteElements.Count;
        TransportRouteElement element = RouteIndex == -1 ? null : _transportRoute.TransportRouteElements[routeIndex];
        yield return _transportController.Unload(element);

        // Load Products
        routeIndex = (RouteIndex + 1) % _transportRoute.TransportRouteElements.Count;
        element = _transportRoute.TransportRouteElements[routeIndex];
        yield return _transportController.Load(element);

        base.OnArrive();
    }

    #endregion
}