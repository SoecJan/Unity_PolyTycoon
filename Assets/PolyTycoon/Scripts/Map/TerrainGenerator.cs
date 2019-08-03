using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public interface ITerrainGenerator
{
    /// <summary>
    /// Translates a worldcoordinate to a ChunkVec2 that can be used in GetTerrainChunk(Vec2)
    /// </summary>
    /// <param name="xPos"></param>
    /// <param name="zPos"></param>
    /// <returns>Vec2 of the TerrainChunk that contains the given positions</returns>
    Vector2 GetTerrainChunkPosition(float xPos, float zPos);

    /// <summary>
    /// Gets a TerrainChunk at a specified Index.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns>The TerrainChunk at the specified Index. May be null</returns>
    TerrainChunk GetTerrainChunk(Vector2 coordinates);

    /// <summary>
    /// Checks to see if all visible TerrainChunks received their Mesh
    /// </summary>
    /// <returns>true if all TerrainChunks received their mesh</returns>
    bool IsReady();

    /// <summary>
    /// Rises or Lowers the Terrain to align with any placed object
    /// </summary>
    /// <param name="terrainChunk"></param>
    /// <param name="objectPosition"></param>
    /// <returns>Returns true on successful Terrain Transformation</returns>
    bool TransformTerrainMeshToObject(SimpleMapPlaceable mapPlaceable);

    TerrainChunk GetChunk(float x, float z);
    bool IsSuitedTerrain(TerrainGenerator.TerrainType terrainType, Vector3 position);
    bool IsSuitedTerrain(TerrainGenerator.TerrainType terrainType, float x, float z);
}

public class TerrainGenerator : MonoBehaviour, ITerrainGenerator
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

    const float
        viewerMoveThresholdForChunkUpdate = 25f; // Distance for the viewer to travel until chunkupdate is invoked

    const float sqrViewerMoveThresholdForChunkUpdate =
        viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;
    public BiomeSettings biomeSettings;

    public Transform viewer;
    public Material mapMaterial;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    float meshWorldSize;
    int chunksVisibleInViewDst;

    public Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    [SerializeField] private bool _useCollider;
    [SerializeField] private Material testMaterial;
    [SerializeField] private float m_terrainPlaceableHeight = 0.08534841f;

    [FormerlySerializedAs("m_terrainHeightTolerance")] [SerializeField]
    private float _terrainHeightTolerance = 0.01f;

    [SerializeField] private float m_terrainAlignOffset = -0.01f;

    [SerializeField] private GameObject _waterMeshPrefab;

    #endregion

    #region Getter & Setter

    public float TerrainPlaceableHeight => m_terrainPlaceableHeight;

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
        if (terrainChunkDictionary.ContainsKey(coordinates))
        {
            return terrainChunkDictionary[coordinates];
        }

        return null;
    }

    /// <summary>
    /// Checks to see if all visible TerrainChunks received their Mesh
    /// </summary>
    /// <returns>true if all TerrainChunks received their mesh</returns>
    public bool IsReady()
    {
        if (visibleTerrainChunks.Count == 0) return false;
        try
        {
            foreach (TerrainChunk visibleChunk in visibleTerrainChunks)
            {
//			Debug.Log("Visible Chunk Ready Check");
                if (!visibleChunk.HasMesh() || visibleChunk.HeightMap.values == null)
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
//		Vector3 position = new Vector3(x, 0f, z);
//		return terrainType == TileType(terrainChunk.meshFilter.sharedMesh.vertices, position);
        TerrainType tileType = TileType(terrainChunk.HeightMap, x, z);
//		Debug.Log(tileType);
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
        float tileY0 = heightMap.values[xIndex + 1, zIndex];
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
                        mapPlaceable.transform.position.y - objectBottomHeight + m_terrainAlignOffset,
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

    // Called at the start
    void Start()
    {
        textureSettings.ApplyToMaterial(mapMaterial);
        textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

        UpdateVisibleChunks();
    }

    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            textureSettings.ApplyToMaterial(mapMaterial);
        }
    }

    // Called every frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) textureSettings.ApplyToMaterial(mapMaterial);

        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        // If viewer position has changed -> call chunk.UpdateCollisionMesh()
        if (_useCollider && viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        // If the change in position was big enough for our threshhold to trigger -> call UpdateVisibleChunks()
        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    public void SetBiome(string biomeName)
    {
        foreach (TerrainChunk chunk in visibleTerrainChunks)
        {
            BiomeGenerator.Biome parsed_enum =
                (BiomeGenerator.Biome) System.Enum.Parse(typeof(BiomeGenerator.Biome), biomeName);
            chunk.ShowBiome(parsed_enum);
        }
    }

    #endregion

    #region Chunk Handling

    // Called if an additional Chunk needs to be loaded
    private void UpdateVisibleChunks()
    {
        // Create empty Hashset of Chunk Vector2, Loop though visibeTerrainChunks and add their Vector2.

        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        // Loop through all visiblechunk Vector2
        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                // Loop through Dictionary to find chunk coordinates
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    // Found an instantiated Chunk, call UpdateTerrain Chunk
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else
                    {
                        // Didn't find an existing chunk -> create a new one and add it to our Dictionary
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings,
                            biomeSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial,
                            _waterMeshPrefab);
                        terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                        newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged; // Subscribe to Event
                        newChunk.Load();
                    }
                }
            }
        }
    }

    // Callback for a change in Visibility of a chunk
    void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }

    #endregion
}

[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int lod;

    public float visibleDstThreshold;

    public float sqrVisibleDstThreshold
    {
        get { return visibleDstThreshold * visibleDstThreshold; }
    }
}