using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator2D
{
    private readonly MeshSettings _meshSettings;

    private readonly int _previewSize = 9; // Needs to have a clean Sqrt
    private readonly List<TerrainChunk2D> _mapChunk2Ds;

    // Start is called before the first frame update
    public TerrainGenerator2D(Transform transform)
    {
        _meshSettings = Resources.Load<MeshSettings>(Util.PathTo("MeshSettings"));
        MapSettings = Resources.Load<HeightMapSettings>(Util.PathTo("HeightMapSettings"));
        _mapChunk2Ds = new List<TerrainChunk2D>();
        for (int i = 0; i < _previewSize; i++)
        {
            GameObject mapSegmentObject = new GameObject("MapSegment: " + i);
            mapSegmentObject.transform.SetParent(transform);
            TerrainChunk2D terrainChunk2D = mapSegmentObject.AddComponent<TerrainChunk2D>();
            _mapChunk2Ds.Add(terrainChunk2D);
        }

        RecalculateChunk();
    }

    public TerrainGenerator2D(Transform transform, int previewSize) : this(transform)
    {
        this._previewSize = previewSize;
    }

    public HeightMapSettings MapSettings { get; private set; }

    public void RecalculateChunk()
    {
        int index = 0;
        int columnCount = (int) Math.Round(Math.Sqrt(_previewSize));
        int offset = columnCount / 2;
        for (int y = -offset; y < columnCount - offset; y++)
        {
            for (int x = -offset; x < columnCount - offset; x++)
            {
                Vector2 sampleCentre = new Vector2(x * 48, y * 48);
                ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(
                    _meshSettings.numVertsPerLine, _meshSettings.numVertsPerLine,
                    MapSettings, sampleCentre), _mapChunk2Ds[index].OnHeightMapReceive);
                index++;
            }
        }
    }
}