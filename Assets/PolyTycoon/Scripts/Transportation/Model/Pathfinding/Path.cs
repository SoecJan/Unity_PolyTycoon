using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wrapper that connects multiple waypoints inside a <see cref="TransportRouteElement"/>.
/// </summary>
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

/// <summary>
/// A WayPoint consists of multiple Vector3s that make up a Path from one position to another.
/// </summary>
public struct WayPoint
{
    private Vector3[] _traversalVectors;

    /// <summary>
    /// Straight line
    /// </summary>
    /// <param name="fromVector3">Start position</param>
    /// <param name="toVector3">End position</param>
    public WayPoint(Vector3 fromVector3, Vector3 toVector3)
    {
        _traversalVectors = new Vector3[2];
        _traversalVectors[0] = fromVector3;
        _traversalVectors[1] = toVector3;
        Radius = 0f;
    }

    /// <summary>
    /// Curve
    /// </summary>
    /// <param name="fromVector3">Start position</param>
    /// <param name="offsetVector3">The offset that forms the curve</param>
    /// <param name="toVector3">End position</param>
    /// <param name="radius">The radius of this curve</param>
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