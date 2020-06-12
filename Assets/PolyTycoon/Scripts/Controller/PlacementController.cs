using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// This class handles all user input to place a <see cref="SimpleMapPlaceable"/> inside the Map.
/// Collisions are handled by <see cref="BuildingManager"/>.
/// The Ground alignment is checked by <see cref="TerrainGenerator"/>.
/// </summary>
public class PlacementController : IPlacementController
{
    #region Attributes

    public static System.Action<BuildingData> _onObjectPlacement;
    private IBuildingManager _buildingManager;
    private ITerrainGenerator _terrainGenerator; // Needed to check the ground below a building before placement
    private TreeManager _treeManager;
    private BuildingData _buildingData;
    #endregion

    #region Getter & Setter

    public BuildingData PlaceableObjectPrefab
    {
        get => _buildingData;
        set
        {
            _buildingData = value;
        }
    }

    #endregion

    #region Default Methods

    public PlacementController(IBuildingManager buildingManager, ITerrainGenerator terrainGenerator)
    {
        _buildingManager = buildingManager;
        _terrainGenerator = terrainGenerator;
    }

    #endregion

    #region Before Placement

    public Vector3 MoveObject(Vector3 targetPosition, Vector3 currentPosition, Vector3 offset)
    {
        return new Vector3(Mathf.Round(targetPosition.x), 0.2572412f + 0.5f, Mathf.Round(targetPosition.z)) + offset;
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
        TerrainChunk terrainChunk = _terrainGenerator.GetChunk(position.x, position.z);
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
    public bool PlaceObject(SimpleMapPlaceable placeableObject)
    {
        // Get all needed references
        float objectBottomHeight = placeableObject.GetHeight() / 2f;
        float yOffset = _terrainGenerator?.TerrainPlaceableHeight ?? 0f;

        GameObject placeableObjectGameObject = placeableObject.gameObject;
        var position = placeableObjectGameObject.transform.position;
        position = new Vector3(position.x, objectBottomHeight + yOffset, position.z);
        placeableObjectGameObject.transform.position = position;

        if (placeableObject && IsPlaceable(placeableObject.transform.position, placeableObject.UsedCoordinates))
        {
            _buildingManager.AddMapPlaceable(placeableObject);
            Vector2 chunkVec = _terrainGenerator.GetTerrainChunkPosition(position.x, position.z);
            TerrainChunk terrainChunk = _terrainGenerator.GetTerrainChunk(chunkVec);
            placeableObject.transform.parent = terrainChunk.meshObject.transform;
            terrainChunk.RemoveEnvironment(position, placeableObject);
        }
        else
        {
            _onObjectPlacement?.Invoke(null);
            _buildingData = null;
            return false;
        }
        _onObjectPlacement?.Invoke(_buildingData);
        return true;
    }

    public bool IsPlaceable(Vector3 position, List<NeededSpace> neededSpaces)
    {
        bool freeSpace = _buildingManager.IsPlaceable(position, neededSpaces);
        bool terrainFlat = IsSuitableTerrain(position, neededSpaces);
        return freeSpace && terrainFlat;
    }

    private bool IsSuitableTerrain(Vector3 positionOffset, IEnumerable<NeededSpace> neededSpaces)
    {
        foreach (NeededSpace neededSpace in neededSpaces)
        {
            if (!_terrainGenerator.IsSuitedTerrain(neededSpace.TerrainType,
                neededSpace.UsedCoordinate + positionOffset))
            {
                return false;
            }
        }

        return true;
    }

    #endregion
}