using System.Collections.Generic;
using UnityEngine;

public struct ThreadsafePlaceable
{
    private Vector3 _position;
    private List<NeededSpace> _neededSpaces;
    private object _payload;

    public ThreadsafePlaceable(SimpleMapPlaceable mapPlaceable, Vector3 startPosition, object payload = null)
    {
        MapPlaceable = mapPlaceable;
        _neededSpaces = new List<NeededSpace>();
        _payload = payload;
        foreach (NeededSpace neededSpace in mapPlaceable.UsedCoordinates)
        {
            _neededSpaces.Add(new NeededSpace(neededSpace,
                Vector3Int.FloorToInt(mapPlaceable.transform.localPosition)));
        }
        _position = startPosition;
    }

    public ThreadsafePlaceable(ComplexMapPlaceable mapPlaceable, Vector3 startPosition, object payload = null)
    {
        MapPlaceable = mapPlaceable;
        _neededSpaces = new List<NeededSpace>();
        _payload = payload;
        _position = startPosition;
        if (mapPlaceable.ChildMapPlaceables.Count == 0) return;
        foreach (SimpleMapPlaceable childMapPlaceable in mapPlaceable.ChildMapPlaceables)
        {
            foreach (NeededSpace neededSpace in childMapPlaceable.UsedCoordinates)
            {
                _neededSpaces.Add(new NeededSpace(neededSpace,
                    Vector3Int.FloorToInt(childMapPlaceable.transform.localPosition)));
            }
        }
    }

    public List<NeededSpace> NeededSpaces
    {
        get => _neededSpaces;
        set => _neededSpaces = value;
    }

    public Vector3 Position => _position;

    public object Payload
    {
        get => _payload;
        set => _payload = value;
    }

    public MapPlaceable MapPlaceable { get; private set; }

    public void Translate(float x, float y, float z)
    {
        _position += new Vector3(x, y, z);
    }
}