using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "PolyTycoon/GameData", order = 1)]
public class GameData : ScriptableObject
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
}

[Serializable]
class PlacementData
{
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _rotation;
}

[Serializable]
class CityData : PlacementData
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
class RouteData
{
    [SerializeField] private string _routeName;
    [SerializeField] private PathType _pathType;
    [SerializeField] private TransportVehicleData _vehicle;
}