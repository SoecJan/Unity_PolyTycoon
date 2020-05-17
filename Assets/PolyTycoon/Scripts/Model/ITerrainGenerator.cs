using UnityEngine;

public interface ITerrainGenerator
{
    float TerrainPlaceableHeight { get;  }

    void Update();

    bool IsReady(Vector3 position);
    
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