using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Holds a reference to all buildings contained in this cityPlaceable. Represents a wrapper for the city.
/// </summary>
public class CityPlaceable : ComplexMapPlaceable, IProductReceiver, IProductEmitter, ICityPlaceable
{
    #region Attributes

    public static System.Action<int, CityPlaceable> _OnCityLevelChange;

    private static List<int> _usedNamesList; // Used indices for city names. Needed for finding unique names.
    private Dictionary<ProductData, ProductStorage> _receivedProducts; // Products that can be received by this city
    private Dictionary<ProductData, int> _productPrices;
    private List<ProductStorage> _emittedProducts; // Products this city emits

    [SerializeField] private CityMainBuilding _mainBuilding; // The main building that is target of pathfinding
    [SerializeField] private List<ProductData> _producedProducts; // The products produced by this city

    private Dictionary<PathFindingNode, Path> _paths; // Paths that were found to and from this city
    private static MoneyUiView _moneyUiView;
    private int _level = 0;
    private int _experiencePoints;

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

    public System.Action<bool> onVisibilityChange;

    /// <summary>
    /// The Main Building that is the target of PathFinding for this city
    /// </summary>
    public CityMainBuilding MainBuilding
    {
        get => _mainBuilding;

        set => _mainBuilding = value;
    }

    public int Level
    {
        get => _level;
        set => _level = value;
    }

    public int ExperiencePoints
    {
        get => _experiencePoints;
        set => _experiencePoints = value;
    }

    #endregion

    #region Methods

    new void Awake()
    {
        if (_usedNamesList == null)
            _usedNamesList = new List<int>();
        name = GetUniqueCityName();
        _receivedProducts = new Dictionary<ProductData, ProductStorage>();
        _emittedProducts = new List<ProductStorage>();
        FillEmittedProducts();
        FillReceivedProducts();
    }

    void Start()
    {
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
                CityMainBuilding building = simpleMapPlaceable.GetComponent<CityMainBuilding>();
                if (!_mainBuilding && building) _mainBuilding = building;
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

        TimeScaleView._onDayOver += ProductAmountReset;

        GetComponentInChildren<Renderer>().gameObject.AddComponent<OnVisibilityChangeCallback>().OnVisibilityChange +=
            isVisible => onVisibilityChange?.Invoke(isVisible);
        _OnCityLevelChange?.Invoke(Level, this);
    }

    private void ProductAmountReset(int day)
    {
        // TODO: Add Value to each product 
        int difference = 0;
        foreach (ProductStorage productStorage in _receivedProducts.Values)
        {
//            Debug.Log(productStorage.StoredProductData.ProductName + " _receivedProducts: " + productStorage.Amount);
            difference += productStorage.Amount;
            productStorage.SetAmount(0);
        }

        foreach (ProductStorage productStorage in _emittedProducts)
        {
            difference += productStorage.MaxAmount - productStorage.Amount;
            productStorage.SetAmount(productStorage.MaxAmount);
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
            _emittedProducts.Add(storage);
            storage.OnAmountChange += OnAmountChange;
        }
    }

    /// <summary>
    /// Adds products that this city can receive to _receivedProducts
    /// </summary>
    private void FillReceivedProducts()
    {
        ProductData productData = FindObjectOfType<ProductManager>().GetRandomCityConsumeableProduct();
        if (_receivedProducts.ContainsKey(productData))
        {
            _receivedProducts[productData].MaxAmount += 5;
        }
        else
        {
            ProductStorage storage = new ProductStorage(productData, 5);
            storage.OnAmountChange += delegate(ProductStorage productStorage, int i)
            {
                if (!_moneyUiView) _moneyUiView = FindObjectOfType<MoneyUiView>();
                if (i <= 0)
                    return; // Because the player gets Money by delivery and does not loose money on restock
                _moneyUiView.ChangeValueBy(_productPrices[productStorage.StoredProductData] * i);
            };
            storage.OnAmountChange += OnAmountChange;
            _receivedProducts.Add(productData, storage);
        }
    }

    private void OnAmountChange(ProductStorage productStorage, int change)
    {
        if (change <= 0) return;
        ExperiencePoints += change;
        if (GetLevelFromExp(ExperiencePoints, Level) > Level)
        {
            Level = (int) GetLevelFromExp(ExperiencePoints, Level);
            _OnCityLevelChange?.Invoke(Level, this);
        }
    }

    #endregion

    public static float GetLevelFromExp(int experiencePoints, int currentLevel)
    {
        // 0 <= (0 * 0 * 100) = 0
        // 5 <= (5 * 5 * 100) = 2500
        // 10 <= (10 * 10 * 100) = 10000
        return experiencePoints <= (currentLevel * currentLevel * 100) ? currentLevel : currentLevel + 1;
    }
}