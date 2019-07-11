using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public List<WayPoint> WayPoints { get; private set; }
   
    public Path()
    {
        WayPoints = new List<WayPoint>();
    }

    public override string ToString()
    {
        string output = "Path: ";
        foreach (WayPoint wayPoint in WayPoints)
        {
            output += wayPoint.ToString() + " ;; ";
        }
        return output;
    }
}

public struct WayPoint
{
    private Vector3[] _traversalVectors;

    public WayPoint(Vector3 fromVector3, Vector3 toVector3)
    {
        _traversalVectors = new Vector3[2];
        _traversalVectors[0] = fromVector3;
        _traversalVectors[1] = toVector3;
        Radius = 0f;
    }

    public WayPoint(Vector3 fromVector3, Vector3 offsetVector3, Vector3 toVector3, float radius)
    {
        _traversalVectors = new Vector3[3];
        _traversalVectors[0] = fromVector3;
        _traversalVectors[1] = offsetVector3;
        _traversalVectors[2] = toVector3;
        Radius = radius;
    }

    public Vector3[] TraversalVectors {
        get => _traversalVectors;
        set => _traversalVectors = value;
    }

    public float Radius { get; private set; }

    public override string ToString()
    {
        string output = "TraversalVectors: ";
        foreach (Vector3 traversalVector in _traversalVectors)
        {
            output += traversalVector.ToString() + ", ";
        }
        return output;
    }
}