using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "PolyTycoon/GameData", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("Map Generation")] 
    [SerializeField] private int _mapSize = 2;
    [SerializeField] private int _mapSeed = 135;
    [SerializeField] private float _mapLacunarity = 1.67f;

    [Header("Player settings")] 
    [SerializeField] private long _budget = 100000;
    [SerializeField] private float _revenueMultiplier = 1.0f;
    [SerializeField] private List<int> _dailyIncomeHistory;
    [SerializeField] private string _playerName = "Default Company";
    [SerializeField] private Color _playerColor = Color.blue;

    [Header("Unlocks")] 
    [SerializeField] private BuildingData[] _unlockedObjects;

    [Header("Chunk data")] [SerializeField]
    private Dictionary<Vector2Int, List<PlacementData>> _chunkChangeData;
    [SerializeField] private List<CityData> _cityList;
    [SerializeField] private List<PlacementData> _treeList;
    [SerializeField] private List<BuildingData> _placedObjects;

    [Header("Route data")] 
    [SerializeField] private List<RouteData> _routeList;


    public int MapSize
    {
        get => _mapSize;
        set => _mapSize = value;
    }

    public int MapSeed
    {
        get => _mapSeed;
        set => _mapSeed = value;
    }

    public float MapLacunarity
    {
        get => _mapLacunarity;
        set => _mapLacunarity = value;
    }

    public long Budget
    {
        get => _budget;
        set => _budget = value;
    }

    public float RevenueMultiplier
    {
        get => _revenueMultiplier;
        set => _revenueMultiplier = value;
    }

    public List<int> DailyIncomeHistory
    {
        get => _dailyIncomeHistory;
        set => _dailyIncomeHistory = value;
    }

    public string PlayerName
    {
        get => _playerName;
        set => _playerName = value;
    }

    public Color PlayerColor
    {
        get => _playerColor;
        set => _playerColor = value;
    }

    public BuildingData[] UnlockedObjects
    {
        get => _unlockedObjects;
        set => _unlockedObjects = value;
    }

    public Dictionary<Vector2Int, List<PlacementData>> ChunkChangeData
    {
        get => _chunkChangeData;
        set => _chunkChangeData = value;
    }

    public List<CityData> CityList
    {
        get => _cityList;
        set => _cityList = value;
    }

    public List<PlacementData> TreeList
    {
        get => _treeList;
        set => _treeList = value;
    }

    public List<BuildingData> PlacedObjects
    {
        get => _placedObjects;
        set => _placedObjects = value;
    }

    public List<RouteData> RouteList
    {
        get => _routeList;
        set => _routeList = value;
    }
}

[Serializable]
public class PlacementData
{
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _rotation;
}

[Serializable]
public class CityData : PlacementData
{
    [SerializeField] private string _cityName = "Default City";
    [SerializeField] private int _citizenCount = 10000;
    [SerializeField] private List<NeededProduct> _neededProducts;
    [SerializeField] private List<ProductData> _producedProducts;
}

[Serializable]
class FactoryData : PlacementData
{
    [SerializeField] private BuildingData _buildingData;
}

[Serializable]
public class RouteData
{
    [SerializeField] private string _routeName;
    [SerializeField] private PathType _pathType;
    [SerializeField] private TransportVehicleData _vehicle;
}