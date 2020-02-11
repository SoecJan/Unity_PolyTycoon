using System.Collections.Generic;
using UnityEngine;

public struct ThreadsafePlaceable
{
    private Vector3 _position;
    private List<NeededSpace> _neededSpaces;

    public ThreadsafePlaceable(SimpleMapPlaceable mapPlaceable, Vector3 startPosition)
    {
        MapPlaceable = mapPlaceable;
        _neededSpaces = new List<NeededSpace>();
        foreach (NeededSpace neededSpace in mapPlaceable.UsedCoordinates)
        {
            _neededSpaces.Add(new NeededSpace(neededSpace,
                Vector3Int.FloorToInt(mapPlaceable.transform.localPosition)));
        }
        _position = startPosition;
    }

    public ThreadsafePlaceable(ComplexMapPlaceable mapPlaceable, Vector3 startPosition)
    {
        MapPlaceable = mapPlaceable;
        _neededSpaces = new List<NeededSpace>();
        foreach (SimpleMapPlaceable childMapPlaceable in mapPlaceable.ChildMapPlaceables)
        {
            foreach (NeededSpace neededSpace in childMapPlaceable.UsedCoordinates)
            {
                _neededSpaces.Add(new NeededSpace(neededSpace,
                    Vector3Int.FloorToInt(childMapPlaceable.transform.localPosition)));
            }
        }

        _position = startPosition;
    }

    public List<NeededSpace> NeededSpaces => _neededSpaces;

    public Vector3 Position => _position;

    public MapPlaceable MapPlaceable { get; private set; }

    public void Translate(float x, float y, float z)
    {
        _position += new Vector3(x, y, z);
    }
}