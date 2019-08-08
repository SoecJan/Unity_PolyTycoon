using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public interface IPlacementManager
{
    MapPlaceable PlaceableObjectPrefab { set; }
    BuildingManager BuildingManager { get; }
    TerrainGenerator TerrainGenerator { get; set; }
    bool PlaceObject(ComplexMapPlaceable complexMapPlaceable);
    bool IsPlaceable(Vector3 position, List<NeededSpace> neededSpaces);
}

/// <summary>
/// This class handles all user input to place a <see cref="SimpleMapPlaceable"/> inside the Map.
/// Collisions are handled by <see cref="BuildingManager"/>.
/// The Ground alignment is checked by <see cref="TerrainGenerator"/>.
/// </summary>
public class PlacementManager : MonoBehaviour, IPlacementManager
{
    #region Attributes

    [SerializeField] private MapPlaceable[] _infrastructurePlaceables; // Objects that can be placed
    [SerializeField] private MapPlaceable[] _productionPlaceables; // Objects that can be placed

    [SerializeField]
    private TerrainGenerator _terrainGenerator; // Needed to check the ground belpw a building before placement

    [SerializeField] private LayerMask _buildingsMask; // Needed to determine the objects to place _buildings on
    [SerializeField] private Vector3 _offsetVec3 = new Vector3(0.5f, 0, 0.5f); // Needed to align _buildings correctly
    [SerializeField] private KeyCode _rotateHotKey = KeyCode.R; // Needed to rotate a selected Building
    [SerializeField] private float _animationSpeedMultiplier = 2f; // Needed to animate a selected Building
    [SerializeField] private float _rotateAmount = 90f; // The Amount to rotate a building on _rotateHotKey press

    private Camera _mainCamera;
    private UserInformationPopup _userInformationPopup;
    private MapPlaceable _currentPlaceableObject; // Object that is being placed
    private Dictionary<Vector2, MapPlaceable> _draggedGameObjects;
    private bool _isDragging;

    #endregion

    #region Getter & Setter

    public MapPlaceable PlaceableObjectPrefab
    {
        set
        {
            if (_currentPlaceableObject && value) Destroy(_currentPlaceableObject);
            _currentPlaceableObject = Instantiate(value);
        }
    }

    public BuildingManager BuildingManager { get; private set; }

    public TerrainGenerator TerrainGenerator
    {
        get => _terrainGenerator;

        set => _terrainGenerator = value;
    }
    public MapPlaceable[] InfrastructurePlaceables => _infrastructurePlaceables;
    
    public MapPlaceable[] ProductionPlaceables => _productionPlaceables;

    #endregion

    #region Default Methods

    void Awake()
    {
        BuildingManager = new BuildingManager();
        _draggedGameObjects = new Dictionary<Vector2, MapPlaceable>();
        _userInformationPopup = FindObjectOfType<UserInformationPopup>();
        _mainCamera = Camera.main;
    }

    //void OnGUI()
    //{
    //	for (int i = 0; i < Buildings.Length; i++)
    //	{
    //		if (!GUI.Button(new Rect(Screen.width / 20, Screen.height / 15 + Screen.height / 12 * i, 100, 30), Buildings[i].name)) continue;
    //		if (_currentPlaceableObject) Destroy(_currentPlaceableObject);
    //		PlaceableObjectPrefab = Buildings[i];
    //	}
    //}

    void Update()
    {
        if (_currentPlaceableObject == null) return;

        MoveObject();
        RotateObject();
        HandleInput();
    }

    #endregion

    #region Before Placement

    void MoveObject()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        // Get current MousePosition
        if (!Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _buildingsMask)) return;
        hitInfo.point -=
            new Vector3(_offsetVec3.x, 0f,
                _offsetVec3.z); // Align the current cursor position to the offset, but keep object height
        float animationHeightOffset = ((1 + Mathf.Sin(Time.time * _animationSpeedMultiplier)) / 2) + 0.5f;
        Vector3 position = new Vector3(Mathf.Round(hitInfo.point.x), hitInfo.point.y + animationHeightOffset,
                               Mathf.Round(hitInfo.point.z)) + _offsetVec3;
        // Check position change
        if (Math.Abs(position.x - _currentPlaceableObject.transform.position.x) > 0.5f ||
            Math.Abs(position.z - _currentPlaceableObject.transform.position.z) > 0.5f)
        {
            SimpleMapPlaceable mapPlaceable = _currentPlaceableObject.GetComponent<SimpleMapPlaceable>();
            if (mapPlaceable && mapPlaceable.IsDraggable && _isDragging)
            {
                Vector3 currentPlaceablePosition = _currentPlaceableObject.transform.position;
                Vector2 key = new Vector2(currentPlaceablePosition.x, currentPlaceablePosition.z);
                if (!_draggedGameObjects.ContainsKey(key))
                {
                    // Add a new draggable to _draggedGameObject
                    Transform currentPlaceableTransform = _currentPlaceableObject.transform;
                    _draggedGameObjects.Add(key,
                        Instantiate(_currentPlaceableObject, currentPlaceableTransform.position,
                            currentPlaceableTransform.rotation));
                }
            }
        }

        foreach (SimpleMapPlaceable previewGameObject in _draggedGameObjects.Values)
        {
            Transform previewGameObjectTransform = previewGameObject.transform;
            Vector3 previewGameObjectPosition = previewGameObjectTransform.position;
            previewGameObjectPosition = new Vector3(previewGameObjectPosition.x,
                hitInfo.point.y + animationHeightOffset, previewGameObjectPosition.z);
            previewGameObjectTransform.position = previewGameObjectPosition;
        }

        _currentPlaceableObject.transform.position = position;
    }

    void RotateObject()
    {
        // Rotate selected Object
        if (Input.GetKeyDown(_rotateHotKey))
        {
            _currentPlaceableObject.Rotate(Vector3.up, _rotateAmount);
        }
    }

    void HandleInput()
    {
        // Place selected Object on left click
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            _isDragging = true;
        }
        else if (_isDragging && Input.GetMouseButtonUp(0))
        {
            if (_currentPlaceableObject.IsDraggable)
            {
                for (int i = _draggedGameObjects.Count - 1; i >= 0; i--)
                {
                    SimpleMapPlaceable previewObject =
                        _draggedGameObjects.Values.ElementAt(i).GetComponent<SimpleMapPlaceable>();
                    if (!PlaceObject(previewObject))
                    {
                        Destroy(previewObject.gameObject);
                    }
                }

                _draggedGameObjects.Clear();
            }

            if (_currentPlaceableObject is ComplexMapPlaceable &&
                !PlaceObject((ComplexMapPlaceable) _currentPlaceableObject))
            {
                Destroy(_currentPlaceableObject.gameObject);
            }
            else if (_currentPlaceableObject is SimpleMapPlaceable && !PlaceObject((SimpleMapPlaceable)_currentPlaceableObject))
            {
                Destroy(_currentPlaceableObject.gameObject);
            }

            _currentPlaceableObject = null;
            _isDragging = false;
        }

        // Remove selected Object on right click
        if (!Input.GetMouseButtonDown(1)) return;

        if (_currentPlaceableObject != null && (_currentPlaceableObject.IsDraggable))
        {
            for (int i = _draggedGameObjects.Count - 1; i >= 0; i--)
            {
                Destroy(_draggedGameObjects.Values.ElementAt(i).gameObject);
            }
            _draggedGameObjects.Clear();
        }
        Destroy(_currentPlaceableObject.gameObject);
        _currentPlaceableObject = null;
        _isDragging = false;
    }

    #endregion

    #region On Object Placement

    public bool PlaceObject(ComplexMapPlaceable complexMapPlaceable)
    {
        foreach (SimpleMapPlaceable childPlaceable in complexMapPlaceable.ChildMapPlaceables)
        {
            if (childPlaceable && !IsPlaceable(childPlaceable.transform.position, childPlaceable.UsedCoordinates))
            {
                return false;
            }
        }

        Transform complexPlaceableTransform = complexMapPlaceable.transform;
        Vector3 position = complexPlaceableTransform.position;
        TerrainChunk terrainChunk = TerrainGenerator.GetChunk(position.x, position.z);
        complexMapPlaceable.transform.parent = terrainChunk.meshObject.transform;
        foreach (SimpleMapPlaceable simpleMapPlaceable in complexMapPlaceable.ChildMapPlaceables)
        {
            PlaceObject(simpleMapPlaceable);
            simpleMapPlaceable.transform.parent = complexPlaceableTransform;
        }

        return true;
    }

    /// <summary>
    /// Places a MapPlaceable in the world. Updates the MapTerrain, places the object as a child of the TerrainChunk. Registers the Object on BuildingManager.
    /// </summary>
    /// <param name="placeableObject"></param>
    /// <returns></returns>
    private bool PlaceObject(SimpleMapPlaceable placeableObject)
    {
        // Get all needed references
        float objectBottomHeight = placeableObject.GetHeight() / 2f;
        float yOffset = TerrainGenerator ? TerrainGenerator.TerrainPlaceableHeight : 0f;

        GameObject placeableObjectGameObject = placeableObject.gameObject;
        var position = placeableObjectGameObject.transform.position;
        position = new Vector3(position.x, objectBottomHeight + yOffset, position.z);
        placeableObjectGameObject.transform.position = position;

        if (placeableObject && IsPlaceable(placeableObject.transform.position, placeableObject.UsedCoordinates))
        {
            BuildingManager.AddMapPlaceable(placeableObject);
            Vector2 chunkVec = TerrainGenerator.GetTerrainChunkPosition(position.x, position.z);
            TerrainChunk terrainChunk = TerrainGenerator.GetTerrainChunk(chunkVec);
            placeableObject.transform.parent = terrainChunk.meshObject.transform;
        }
        else
        {
            if (_userInformationPopup)
                _userInformationPopup.InformationText =
                    "Couldn't place Object at " + placeableObject.gameObject.transform.position;
            return false;
        }

        return true;
    }

    public bool IsPlaceable(Vector3 position, List<NeededSpace> neededSpaces)
    {
        bool freeSpace = BuildingManager.IsPlaceable(position, neededSpaces);
        bool terrainFlat = IsSuitableTerrain(position, neededSpaces);
//        Debug.Log(placeableNotNull + "; " + freeSpace + "; " + terrainFlat);
        return freeSpace && terrainFlat;
    }

    private bool IsSuitableTerrain(Vector3 positionOffset, IEnumerable<NeededSpace> neededSpaces)
    {
        foreach (NeededSpace neededSpace in neededSpaces)
        {
            if (!TerrainGenerator.IsSuitedTerrain(neededSpace.TerrainType,
                neededSpace.UsedCoordinate + positionOffset))
            {
                return false;
            }
        }

        return true;
    }

//	private Dictionary<BiomeGenerator.Biome, float> GetBiomeValue(SimpleMapPlaceable mapPlaceable)
//	{
//		TerrainChunk terrainChunk = GetChunk(mapPlaceable.transform.position.x, mapPlaceable.transform.position.z);
//		Vector2 pos = new Vector2(Mathf.FloorToInt(mapPlaceable.transform.position.x + 22.5f), Mathf.FloorToInt(mapPlaceable.transform.position.z + 22.5f));
//		Dictionary<BiomeGenerator.Biome, float> biomeValueDictionary = new Dictionary<BiomeGenerator.Biome, float>();
//		foreach (BiomeGenerator.Biome existingBiome in Enum.GetValues(typeof(BiomeGenerator.Biome)))
//		{
//			if (existingBiome.Equals(BiomeGenerator.Biome.None)) continue;
//			BiomeData biomeData = terrainChunk.GetBiomeData(existingBiome);
//			biomeValueDictionary.Add(existingBiome, biomeData.ArrayData[(int)pos.x, (int)pos.y]);
//		}
//		return biomeValueDictionary;
//	}

    #endregion
}