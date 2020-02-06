using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator2D : MonoBehaviour
{
    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;

    [SerializeField] private int _previewSize = 9; // Needs to have a clean Sqrt
    private List<MapChunk2D> _mapChunk2Ds;

    // Start is called before the first frame update
    void Start()
    {
        _mapChunk2Ds = new List<MapChunk2D>();
        for (int i = 0; i < _previewSize; i++)
        {
            GameObject mapSegmentObject = new GameObject("MapSegment: " + i);
            mapSegmentObject.transform.SetParent(transform);
            MapChunk2D mapChunk2D = mapSegmentObject.AddComponent<MapChunk2D>();
            _mapChunk2Ds.Add(mapChunk2D);
        }

        RecalculateMap();
    }

    public void RecalculateMap()
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
                    meshSettings.numVertsPerLine, meshSettings.numVertsPerLine,
                    heightMapSettings, sampleCentre), _mapChunk2Ds[index].OnHeightMapReceive);
                index++;
            }
        }
    }
}