using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This interface describes all functionality a city has. 
/// </summary>
public interface ICityPlaceable
{
    /// <summary>
    /// The main buidling of this city. It is the target of pathfinding for this city. 
    /// </summary>
    CityMainBuilding MainBuilding { get; set; }

    /// <summary>
    /// Count of people living in this cityPlaceable.
    /// </summary>
    /// <returns>Number of inhabitants</returns>
    int CurrentInhabitantCount();

    /// <summary>
    /// Rotates the city. Useful for bringing variation into city generation.
    /// </summary>
    /// <param name="axis">The axis of rotation. Vector3.up for terrain aligned rotation.</param>
    /// <param name="rotationAmount">The amount of rotation. Input of 90f = 90° of rotation.</param>
    void Rotate(Vector3 axis, float rotationAmount);
}

/// <summary>
/// Holds a reference to all buildings contained in this cityPlaceable. Represents a wrapper for the city.
/// </summary>
public class CityPlaceable : ComplexMapPlaceable, IProductReceiver, IProductEmitter, IPathNode, ICityPlaceable
{
    #region Attributes

    private static List<int> _usedNamesList; // Used indices for city names. Needed for finding unique names.
    private Dictionary<ProductData, ProductStorage> _receivedProducts; // Products that can be received by this city
    private Dictionary<ProductData, int> _productPrices;
    private List<ProductStorage> _emittedProducts; // Products this city emits

    [SerializeField] private CityMainBuilding _mainBuilding; // The main building that is target of pathfinding
    [SerializeField] private List<ProductData> _producedProducts; // The products produced by this city

    private Dictionary<PathFindingNode, Path> _paths; // Paths that were found to and from this city

    #endregion

    #region Getter & Setter

    #region ProductReceiver

    public ProductStorage ReceiverStorage(ProductData productData = null)
    {
        if (_receivedProducts.Count == 0) return null;
        if (productData == null && _receivedProducts.Count == 1)
            return _receivedProducts.Values.GetEnumerator().Current;
        return productData != null ? _receivedProducts[productData] : null;
    }

    public List<ProductData> ReceivedProductList()
    {
        return new List<ProductData>(_receivedProducts.Keys);
    }

    #endregion

    #region ProductEmitter

    public ProductStorage EmitterStorage(ProductData productData = null)
    {
        if (productData != null) return _emittedProducts.Find(x => x.StoredProductData.Equals(productData));
        return _emittedProducts.Count == 1 ? _emittedProducts.ToArray()[0] : null;
    }

    public List<ProductData> EmittedProductList()
    {
        return _producedProducts;
    }

    #endregion

    /// <summary>
    /// Count of people living in this cityPlaceable.
    /// </summary>
    /// <returns></returns>
    public int CurrentInhabitantCount()
    {
        return ChildMapPlaceables.Count * 3;
    }

    /// <summary>
    /// The Main Building that is the target of PathFinding for this city
    /// </summary>
    public CityMainBuilding MainBuilding
    {
        get => _mainBuilding;

        set => _mainBuilding = value;
    }

    #endregion

    #region Methods

    void Awake()
    {
        if (_usedNamesList == null)
            _usedNamesList = new List<int>();
        if ("".Equals(BuildingName))
            BuildingName = GetUniqueCityName();
        _receivedProducts = new Dictionary<ProductData, ProductStorage>();
        _emittedProducts = new List<ProductStorage>();
        FillEmittedProducts();
        FillReceivedProducts();
    }

    public override void Start()
    {
        base.Start();

        // Add child components if there are none
        if (ChildMapPlaceables.Count == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                SimpleMapPlaceable simpleMapPlaceable =
                    transform.GetChild(i).gameObject.GetComponent<SimpleMapPlaceable>();
                // Append to the ChildMapPlaceables List
                if (simpleMapPlaceable) ChildMapPlaceables.Add(simpleMapPlaceable);
                // Find the mainbuilding if none is specified
                if (!_mainBuilding && simpleMapPlaceable is CityMainBuilding building) _mainBuilding = building;
            }
        }

        // Initialize data structures
        _paths = new Dictionary<PathFindingNode, Path>();
        _productPrices = new Dictionary<ProductData, int>();
        ProductManager productManager = FindObjectOfType<ProductManager>();
        foreach (ProductData productData in productManager.Products)
        {
            _productPrices.Add(productData,
                (int) (productData.BasePrice *
                       Random.Range(1 - productData.RandomPriceFactor, 1 + productData.RandomPriceFactor)));
        }

        ;
        TimeScaleUi._onDayOver += ProductAmountReset;
    }

    private void ProductAmountReset(int day)
    {
        // TODO: Add Value to each product 
        foreach (ProductStorage productStorage in _receivedProducts.Values)
        {
            Debug.Log(productStorage.StoredProductData.ProductName + " _receivedProducts: " + productStorage.Amount);
//            int difference = productStorage.Amount;
//            _moneyUiController.AddMoney(difference * 100);
            productStorage.Amount = 0;
        }

        foreach (ProductStorage productStorage in _emittedProducts)
        {
            int difference = productStorage.MaxAmount - productStorage.Amount;
//            Debug.Log(productStorage.StoredProductData.ProductName + " _emittedProducts: " + productStorage.Amount + ", " + difference);
//            _moneyUiController.AddMoney(difference * 100);
            productStorage.Amount = productStorage.MaxAmount;
        }
    }

    /// <summary>
    /// Generates a unique name for a city.
    /// Names are randomly taken from: "Assets/PolyTycoon/Resources/Data/CityPlacement/city_name.txt".
    /// </summary>
    /// <returns>the unique name</returns>
    private string GetUniqueCityName()
    {
        TextAsset spellData = Resources.Load("Data/CityPlacement/city_name") as TextAsset;
        string[] names = spellData.text.Split('\n');
        if (_usedNamesList.Count >= names.Length)
        {
            Debug.LogError("All names are taken");
            string overflowName = "Overflow city " + _usedNamesList.Count;
            _usedNamesList.Add(_usedNamesList.Count + 1);
            return overflowName;
        }

        // Loop over all used indices
        int output = Random.Range(0, names.Length);
        while (_usedNamesList.Contains(output))
            output = Random.Range(0, names.Length);
        return names[output];
    }

    /// <summary>
    /// Adds products that this city can emit to _emittedProducts from _producedProducts with a random range.
    /// </summary>
    private void FillEmittedProducts()
    {
        foreach (ProductData product in _producedProducts)
        {
            int amount = Random.Range(3, 7);
            ProductStorage storage = new ProductStorage(product, amount, amount);
//            storage.OnAmountChange += (productStorage, i) =>
//            {
//                _moneyUiController.AddMoney(_productPrices[productStorage.StoredProductData] * i);
//            };
            _emittedProducts.Add(storage);
        }
    }

    /// <summary>
    /// Adds products that this city can receive to _receivedProducts
    /// </summary>
    private void FillReceivedProducts()
    {
        foreach (SimpleMapPlaceable simpleMapPlaceable in ChildMapPlaceables)
        {
            if (!(simpleMapPlaceable is CityBuilding)) continue;
            CityBuilding cityBuilding = ((CityBuilding) simpleMapPlaceable);
            foreach (NeededProduct neededProduct in cityBuilding.ConsumedProducts)
            {
                if (_receivedProducts.ContainsKey(neededProduct.Product))
                {
                    _receivedProducts[neededProduct.Product].MaxAmount += neededProduct.Amount;
                }
                else
                {
                    ProductStorage storage = new ProductStorage(neededProduct.Product, neededProduct.Amount);
                    storage.OnAmountChange += (productStorage, i) =>
                    {
                        _moneyUiController.AddMoney(_productPrices[productStorage.StoredProductData] * i);
                    };
                    _receivedProducts.Add(neededProduct.Product, storage);
                }
            }
        }
    }

    /// <summary>
    /// <inheritdoc cref="IPathNode.PathTo"/>
    /// </summary>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    public Path PathTo(PathFindingNode targetNode)
    {
        return _paths.ContainsKey(targetNode) ? _paths[targetNode] : null;
    }

    /// <summary>
    ///  <inheritdoc cref="IPathNode.AddPath"/>
    /// </summary>
    /// <param name="targetNode"></param>
    /// <param name="path"></param>
    public void AddPath(PathFindingNode targetNode, Path path)
    {
        if (_paths.ContainsKey(targetNode))
        {
            _paths[targetNode] = path;
        }
        else
        {
            _paths.Add(targetNode, path);
        }
    }

    /// <summary>
    ///  <inheritdoc cref="IPathNode.RemovePath"/>
    /// </summary>
    /// <param name="targetNode"></param>
    public void RemovePath(PathFindingNode targetNode)
    {
        _paths.Remove(targetNode);
    }

    #endregion
}