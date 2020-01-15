using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This interface describes functionality fpr entities that want to transport products from a <see cref="IProductEmitter"/>
/// to a <see cref="IProductReceiver"/>.
/// </summary>
public interface ITransport
{
    /// <summary>
    /// The current amount of products loaded
    /// </summary>
    int CurrentCapacity { get; }

    /// <summary>
    /// The maximum amount of products that can be loaded
    /// </summary>
    int MaxCapacity { get; set; }

    /// <summary>
    /// Time it takes to transfer the TransferAmount of items
    /// </summary>
    float TransferTime { get; set; }

    /// <summary>
    /// Amount of items loaded in one timeslot
    /// </summary>
    int TransferAmount { get; set; }

    /// <summary>
    /// The storage of a specified product
    /// </summary>
    /// <param name="productData">The specified product for getting the associated storage</param>
    /// <returns>Storage instance of the specified product</returns>
    ProductStorage TransportStorage(ProductData productData);

    /// <summary>
    /// List of products that are currently in storage
    /// </summary>
    List<ProductData> LoadedProducts { get; }

    /// <summary>
    /// Product loading logic - coroutine
    /// </summary>
    /// <param name="transportRouteElement">The current element that contains the <see cref="TransportRouteSetting"/></param>
    /// <returns></returns>
    IEnumerator Load(TransportRouteElement transportRouteElement);

    /// <summary>
    /// Product unloading logic - coroutine
    /// </summary>
    /// <param name="transportRouteElement">The current element that contains the <see cref="TransportRouteSetting"/>.</param>
    /// <returns></returns>
    IEnumerator Unload(TransportRouteElement transportRouteElement); // 
}

/// <summary>
/// This controller handles the logic of a <see cref="TransportVehicle"/>.
/// </summary>
[Serializable]
public class TransportVehicleController : ITransport
{
    private Dictionary<ProductData, ProductStorage> _transporterStorage; // The Products that are loaded
    public string VehicleName { get; set; }
    public float MaxSpeed { get; set; } = 2f;
    public int CurrentCapacity { get; } = 0;
    public int MaxCapacity { get; set; } = 4;
    public float TransferTime { get; set; } = 1f;
    public int TransferAmount { get; set; } = 1;
    public List<ProductData> LoadedProducts => new List<ProductData>(_transporterStorage.Keys);

    public TransportVehicleController()
    {
        _transporterStorage = new Dictionary<ProductData, ProductStorage>();
    }

    public ProductStorage TransportStorage(ProductData productData)
    {
        return _transporterStorage.ContainsKey(productData) ? _transporterStorage[productData] : null;
    }

    /// <summary>
    /// This function loads products from a <see cref="IProductEmitter"/> into the _transporterStorage.
    /// </summary>
    /// <param name="transportRouteElement">The current <see cref="TransportRouteElement"/> that is used by the <see cref="RouteMover"/>.</param>
    /// <returns></returns>
    public IEnumerator Load(TransportRouteElement transportRouteElement)
    {
        if (!(transportRouteElement.FromNode is IProductEmitter producer)) yield break;
//        Debug.Log(producer.EmitterStorage().StoredProductData.ProductName);
        // Search unloading settings and execute them
        foreach (TransportRouteSetting setting in transportRouteElement.RouteSettings)
        {
            bool isUnload = !setting.IsLoad;
            bool isEmitterProducingProduct = producer.EmitterStorage(setting.ProductData) != null;

            if (isUnload || !isEmitterProducingProduct) continue;

            Debug.Log(setting.ToString());

            if (!_transporterStorage.ContainsKey(setting.ProductData))
            {
                _transporterStorage.Add(setting.ProductData, new ProductStorage(setting.ProductData)
                    {MaxAmount = MaxCapacity, Amount = 0});
            }

            // Handle actual unloading process
            ProductStorage emitterStorage = producer.EmitterStorage(setting.ProductData);
            ProductStorage truckStorage = _transporterStorage[setting.ProductData];
            int loadAmount = setting.Amount;
            while (loadAmount > 0 && truckStorage.Amount != truckStorage.MaxAmount)
            {
                while (producer.EmitterStorage(setting.ProductData).Amount == 0)
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

    public IEnumerator Unload(TransportRouteElement transportRouteElement)
    {
        // Unload Products
        if (transportRouteElement == null || !(transportRouteElement.ToNode is IProductReceiver consumer)) yield break;

        foreach (TransportRouteSetting setting in transportRouteElement.RouteSettings)
        {
            bool isLoad = setting.IsLoad;
            bool isLoadedProductReceiver = consumer.ReceiverStorage(setting.ProductData) != null;
            bool hasProductLoaded = _transporterStorage.ContainsKey(setting.ProductData);
            // if: setting is for loading, receiver not in need of the product or the truck has none of the product
            if (isLoad || !isLoadedProductReceiver || !hasProductLoaded) continue;

            Debug.Log(setting.ToString());

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
                receiverStorage.OnAmountChange?.Invoke(receiverStorage, TransferAmount);
                yield return new WaitForSeconds(TransferTime);
            }

            if (truckStorage.Amount == 0) _transporterStorage.Remove(setting.ProductData);
        }
    }
}

/// <summary>
/// Vehicle that can transport products from one place to the other.
/// </summary>
[RequireComponent(typeof(RouteMover))] // To move this entity
[RequireComponent(typeof(BoxCollider))] // To be able to click on this entity
public class TransportVehicle : MonoBehaviour
{
    #region Attributes

    private TransportVehicleController _transportController; // Logic of this class
    private TransportRoute _transportRoute; // Route this vehicle drives on
    private RouteMover _routeMover; // Mover for the visual representation

    #endregion

    #region Getter & Setter

    public Outline Outline { get; private set; }

    public Sprite Sprite { get; set; }

    public int MaxCapacity
    {
        get => _transportController.MaxCapacity;
        set => _transportController.MaxCapacity = value;
    }

    public string VehicleName
    {
        get => _transportController.VehicleName;
        set => _transportController.VehicleName = value;
    }

    public float MaxSpeed
    {
        get => _transportController.MaxSpeed;
        set => _transportController.MaxSpeed = value;
    }

    public float TransferTime
    {
        get => _transportController.TransferTime;
        set => _transportController.TransferTime = value;
    }

    public List<ProductData> LoadedProducts => _transportController.LoadedProducts;

    public ProductStorage TransportStorage(ProductData productData)
    {
        return _transportController.TransportStorage(productData);
    }

    public static Action<TransportVehicle> OnClickAction { get; set; }

    public TransportRoute TransportRoute
    {
        get => _transportRoute;

        set
        {
            _transportRoute = value;
            _routeMover.PathList = _transportRoute.PathList;
        }
    }

    public RouteMover RouteMover
    {
        get => _routeMover;
        set
        {
            if (_routeMover) _routeMover.OnArrive -= OnArrive;
            _routeMover = value;
            _routeMover.OnArrive += OnArrive;
        }
    }

    #endregion

    #region Methods

    private void Awake()
    {
        _transportController = new TransportVehicleController();
    }

    void Start()
    {
        if (Outline) return;
        Outline = gameObject.AddComponent<Outline>();
        Outline.OutlineMode = Outline.Mode.OutlineAll;
        Outline.OutlineColor = Color.yellow;
        Outline.OutlineWidth = 5f;
        Outline.enabled = false;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            OnClickAction(this);
        }
    }

    private int Modulo(int x, int m)
    {
        return (x % m + m) % m;
    }

    private void OnArrive()
    {
        StartCoroutine(HandleLoad());
    }

    /// <summary>
    /// Handles the product transfer process at arrival on a station
    /// </summary>
    /// <returns>Coroutine</returns>
    private IEnumerator HandleLoad()
    {
        // Unload Products
        int routeIndex = Modulo((_routeMover.PathIndex - 1), _transportRoute.TransportRouteElements.Count);
        TransportRouteElement element = _transportRoute.TransportRouteElements[routeIndex];
        yield return _transportController.Unload(element);

        // Load Products
        routeIndex = (_routeMover.PathIndex) % _transportRoute.TransportRouteElements.Count;
        element = _transportRoute.TransportRouteElements[routeIndex];
        yield return _transportController.Load(element);

        // Done tranferring products -> move on towards the next station
        _routeMover.MoveToNextElement();
    }

    #endregion
}