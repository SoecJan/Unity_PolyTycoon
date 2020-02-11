using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class TerrainGenerator : ITerrainGenerator
{
    #region Attributes

    public enum TerrainType
    {
        Obstructed,
        Mountain,
        Hill,
        Flatland,
        Coast,
        Ocean
    };

    private GameObject _sceneObject;
    private int _maxMapSize;

    private MeshSettings meshSettings;
    private HeightMapSettings heightMapSettings;

    private Transform viewer;
    private Material terrainMaterial;

    private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> _visibleTerrainChunks = new List<TerrainChunk>();

    private const float _terrainPlaceableHeight = 0.2405538f;
    private const float _terrainHeightTolerance = 0.01f;

    #endregion

    public TerrainGenerator(MeshSettings meshSettings, HeightMapSettings heightMapSettings, Transform viewer,
        Material terrainMaterial, int maxMapSize)
    {
        _sceneObject = new GameObject("TerrainGenerator");
        this.meshSettings = meshSettings;
        this.heightMapSettings = heightMapSettings;
        this.viewer = viewer;
        this.terrainMaterial = terrainMaterial;
        this._maxMapSize = maxMapSize;
        UpdateVisibleChunks();
    }

    #region Getter & Setter

    public float TerrainPlaceableHeight => _terrainPlaceableHeight;

    public int MaxMapSize
    {
        get => _maxMapSize;
        set => _maxMapSize = value;
    }

    /// <summary>
    /// Translates a worldcoordinate to a ChunkVec2 that can be used in GetTerrainChunk(Vec2)
    /// </summary>
    /// <param name="xPos"></param>
    /// <param name="zPos"></param>
    /// <returns>Vec2 of the TerrainChunk that contains the given positions</returns>
    public Vector2 GetTerrainChunkPosition(float xPos, float zPos)
    {
        int currentChunkCoordX = Mathf.RoundToInt(xPos / ((float) meshSettings.numVertsPerLine - 3));
        int currentChunkCoordY = Mathf.RoundToInt(zPos / ((float) meshSettings.numVertsPerLine - 3));

        return new Vector2(currentChunkCoordX, currentChunkCoordY);
    }

    /// <summary>
    /// Gets a TerrainChunk at a specified Index.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns>The TerrainChunk at the specified Index. May be null</returns>
    public TerrainChunk GetTerrainChunk(Vector2 coordinates)
    {
        return terrainChunkDictionary.ContainsKey(coordinates) ? terrainChunkDictionary[coordinates] : null;
    }

    public bool IsReady(Vector3 position)
    {
        try
        {
            TerrainChunk terrainChunk = GetChunk(position.x, position.z);
            if (!terrainChunk.HasHeightMap())
            {
                return false;
            }
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks to see if all visible TerrainChunks received their Mesh
    /// </summary>
    /// <returns>true if all TerrainChunks received their mesh</returns>
    public bool IsReady()
    {
        if (_visibleTerrainChunks.Count == 0) return false;
        try
        {
            foreach (TerrainChunk visibleChunk in _visibleTerrainChunks)
            {
                if (!visibleChunk.HasMesh() || !visibleChunk.HasHeightMap())
                {
                    return false;
                }
            }
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// The amount of placement slots in a chunk in one direction. It is not the total amount of slots.
    /// </summary>
    /// <returns>The terrain slot amount</returns>
    private float GetSlotsPerChunk()
    {
        return ((float) meshSettings.numVertsPerLine - 3);
    }

    #endregion

    #region TerrainMesh Manipulation

    /// <summary>
    /// Rises or Lowers the Terrain to align with any placed object
    /// </summary>
    /// <param name="terrainChunk"></param>
    /// <param name="objectPosition"></param>
    /// <returns>Returns true on successful Terrain Transformation</returns>
    public bool TransformTerrainMeshToObject(SimpleMapPlaceable mapPlaceable)
    {
        Vector2 chunkVec =
            GetTerrainChunkPosition(mapPlaceable.transform.position.x, mapPlaceable.transform.position.z);
        TerrainChunk terrainChunk = GetTerrainChunk(chunkVec);
        // Get Terrain Information reference
        MeshFilter meshFilter = terrainChunk.meshFilter;
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = CalculateMeshTransformation(mesh.vertices, mapPlaceable);
        if (vertices != null)
        {
            mesh.vertices = vertices;
            meshFilter.sharedMesh = mesh;
            mapPlaceable.transform.parent = terrainChunk.meshObject.transform;
        }

        return vertices != null;
    }

    public TerrainChunk GetChunk(float x, float z)
    {
        Vector2 chunkVec = GetTerrainChunkPosition(x, z);
        return GetTerrainChunk(chunkVec);
    }

    public bool IsSuitedTerrain(TerrainType terrainType, Vector3 position)
    {
        return IsSuitedTerrain(terrainType, position.x, position.z);
    }

    public bool IsSuitedTerrain(TerrainType terrainType, float x, float z)
    {
        TerrainChunk terrainChunk = GetChunk(x, z);
        TerrainType tileType = TileType(terrainChunk.HeightMap, x, z);
        return tileType == terrainType;
    }

    private TerrainType TileType(HeightMap heightMap, float x, float z)
    {
        int tilePerChunk = (int) GetSlotsPerChunk();
        float positionOffset = 22.5f;

        x += positionOffset;
        x = (((x % tilePerChunk) + tilePerChunk) % tilePerChunk); // Calculate Modulo between 0 and tilePerChunk
        z += positionOffset;
        z = (((z % tilePerChunk) + tilePerChunk) % tilePerChunk);

        int xIndex = (int) x + 1;
        int zIndex = (int) ((tilePerChunk - 1) - z) + 1;

        float tileX0 = heightMap.values[xIndex, zIndex];
        float tileX1 = heightMap.values[xIndex + 1, zIndex];
        float tileY0 = heightMap.values[xIndex, zIndex + 1];
        float tileY1 = heightMap.values[xIndex + 1, zIndex + 1];

        float min = Mathf.Min(tileX0, tileX1, tileY0, tileY1);
        float max = Mathf.Max(tileX0, tileX1, tileY0, tileY1);

        if (Math.Abs(min - max) < 0.1f)
        {
            if (Mathf.Abs(min - TerrainPlaceableHeight) < _terrainHeightTolerance)
            {
                return TerrainType.Flatland;
            }

            if (Mathf.Abs(min - 0) < 0.1f)
            {
                return TerrainType.Ocean;
            }
        }
        else
        {
            if (Mathf.Abs(min - 0) < 0.1f && Mathf.Abs(max - TerrainPlaceableHeight) < _terrainHeightTolerance)
            {
                return TerrainType.Coast;
            }
        }

        return TerrainType.Obstructed;
    }

    private TerrainType TileType(Vector3[] meshVertices, Vector3 position)
    {
        float positionOffset = 22.5f;
        Vector3 placedPosition = Vector3Int.FloorToInt(position);
        placedPosition = placedPosition + new Vector3(0.5f, 0, 0.5f);
        int[] indices = GetMeshIndices((int) (placedPosition.x + positionOffset),
            (int) (placedPosition.z + positionOffset));

        float min = meshVertices[indices[0]].y;
        float max = meshVertices[indices[0]].y;
        for (int i = 1; i < indices.Length; i++)
        {
            if (meshVertices[indices[i]].y > max && indices[i] < meshVertices.Length && indices[i] > 0)
            {
                max = meshVertices[indices[i]].y;
            }
            else if (meshVertices[indices[i]].y < min && indices[i] < meshVertices.Length && indices[i] > 0)
            {
                min = meshVertices[indices[i]].y;
            }
        }

        // Flat terrains
        if (Math.Abs(min - max) < 0.1f)
        {
            if (Mathf.Abs(min - TerrainPlaceableHeight) < _terrainHeightTolerance)
            {
                return TerrainType.Flatland;
            }
            else if (Mathf.Abs(min - 0) < 0.1f)
            {
                return TerrainType.Ocean;
            }
        }
        else
        {
            if (Mathf.Abs(min - 0) < 0.1f && Mathf.Abs(max - TerrainPlaceableHeight) < _terrainHeightTolerance)
            {
                return TerrainType.Coast;
            }
        }

        return TerrainType.Mountain;
    }

    /// <summary>
    /// Finds all Vertices close to the given position.
    /// </summary>
    /// <param name="meshVertices"></param>
    /// <param name="mapPlaceable"></param>
    /// <param name="objectHeight"></param>
    /// <returns>Transformed Mesh Information. May be null</returns>
    private Vector3[] CalculateMeshTransformation(Vector3[] meshVertices, SimpleMapPlaceable mapPlaceable)
    {
        Debug.Log(TileType(meshVertices, mapPlaceable.transform.position));
        // Get information references
        float objectBottomHeight = mapPlaceable.GetHeight() / 2f;
        float positionOffset = 22.5f; // offset to make TerrainChunk bottom left slot Position 0,0

        Vector3 placedPosition = Vector3Int.FloorToInt(mapPlaceable.transform.position);
        placedPosition = placedPosition + new Vector3(0.5f, 0, 0.5f);

        foreach (NeededSpace usedCoordinate in mapPlaceable.UsedCoordinates)
        {
            Vector3 occupiedSpace = placedPosition + usedCoordinate.UsedCoordinate;
            int[] indices = GetMeshIndices((int) (occupiedSpace.x + positionOffset),
                (int) (occupiedSpace.z + positionOffset));
            // Change MeshArray -> Return null if Terrain is not suitable
            foreach (int index in indices)
            {
                if (Mathf.Abs(meshVertices[index].y - TerrainPlaceableHeight) < _terrainHeightTolerance)
                {
                    meshVertices[index] = new Vector3(meshVertices[index].x,
                        mapPlaceable.transform.position.y - objectBottomHeight - 0.01f,
                        meshVertices[index].z);
                }
                else
                {
                    return null;
                }
            }
        }

        return meshVertices;
    }

    /// <summary>
    /// Calculates Indices of a given Slot
    /// </summary>
    /// <param name="x">x tile position in a given terrainChunk</param>
    /// <param name="z">z tile position in a given terrainChunk</param>
    /// <returns>The Indices to find corresponding vertices at</returns>
    private int[] GetMeshIndices(int x, int z)
    {
        // Meshinformation
        int tilePerChunk = (int) GetSlotsPerChunk();
        int vertsPerTile = 6;
        x = (((x % tilePerChunk) + tilePerChunk) % tilePerChunk); // Calculate Modulo between 0 and tilePerChunk
        z = (((z % tilePerChunk) + tilePerChunk) % tilePerChunk);
        int[] meshIndices = new int[vertsPerTile];
        // Normalize Data to align to world coordinates
        int xIndex = x * vertsPerTile;
        int zIndex = ((tilePerChunk - 1) - z) * (vertsPerTile * tilePerChunk);
        // Save all indices
        for (int i = 0; i < meshIndices.Length; i++)
        {
            meshIndices[i] = xIndex + zIndex + i;
        }

        return meshIndices;
    }

    #endregion

    #region Default Methods

    // Called every frame
    public void Update()
    {
        UpdateVisibleChunks();
    }

    #endregion

    #region Chunk Handling

    // Called if an additional Chunk needs to be loaded
    private void UpdateVisibleChunks()
    {
        // Create empty Hashset of Chunk Vector2, Loop though visibleTerrainChunks and add their Vector2.
        HashSet<Vector2> updatedChunkCoords = new HashSet<Vector2>();
        for (int i = _visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            updatedChunkCoords.Add(_visibleTerrainChunks[i].coord);
            _visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewer.position.x / meshSettings.meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewer.position.z / meshSettings.meshWorldSize);

        // Loop through all visible chunks Vector2
        for (int yOffset = -_maxMapSize; yOffset <= _maxMapSize; yOffset++)
        {
            for (int xOffset = -_maxMapSize; xOffset <= _maxMapSize; xOffset++)
            {
                // Loop through Dictionary to find chunk coordinates
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (updatedChunkCoords.Contains(viewedChunkCoord)) continue;

                // Found an instantiated Chunk, call UpdateTerrain Chunk
                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                }
                else if (MaxMapSize == 0
                         || (Mathf.Abs(viewedChunkCoord.x) < MaxMapSize
                             && Mathf.Abs(viewedChunkCoord.y) < MaxMapSize)) // Limit MapGeneration. 0 = Infinite
                {
                    // Didn't find an existing chunk -> create a new one and add it to our Dictionary
                    TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings,
                        _sceneObject.transform, viewer, terrainMaterial);
                    terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                    newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged; // Subscribe to Event
                    newChunk.Load();
                }
            }
        }
    }

    // Callback for a change in Visibility of a chunk
    void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
        {
            _visibleTerrainChunks.Add(chunk);
        }
        else
        {
            _visibleTerrainChunks.Remove(chunk);
        }
    }

    #endregion
}