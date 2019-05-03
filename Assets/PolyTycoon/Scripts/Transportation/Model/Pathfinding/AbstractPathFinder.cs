using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPathFinder : MonoBehaviour
{
    protected IPathFindingAlgorithm _pathFindingAlgorithm;
    public abstract void FindPath(TransportRoute transportRoute, System.Action<TransportRoute> callback);
}

public abstract class Node : IHeapItem<Node>
{
    private Node _parent;
    private int _hCost;
    private int _gCost;

    protected Node(int hCost, int gCost)
    {
        _hCost = hCost;
        _gCost = gCost;
    }

    protected Node(Node parent, int hCost, int gCost) : this(hCost, gCost)
    {
        _parent = parent;
    }
    
    public int HeapIndex { get; set; }
    
    public virtual int FCost {
        get { return HCost + GCost; }
    }

    public int HCost {
        get {
            return _hCost;
        }

        set {
            _hCost = value;
        }
    }

    public int GCost {
        get {
            return _gCost;
        }

        set {
            _gCost = value;
        }
    }
    
    internal Node Parent {
        get {
            return _parent;
        }

        set {
            _parent = value;
        }
    }
    
    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(other.HCost);
        }

        return -compare;
    }
}

public class Path
{
    #region Attributes
    private List<WayPoint> _wayPoints; // Nodes that need to be visited one after the other
    #endregion

    #region Getter & Setter
    public List<WayPoint> WayPoints {
        get {
            return _wayPoints;
        }

        set {
            _wayPoints = value;
        }
    }
    #endregion

    #region Constructor
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

    #endregion
}

public struct WayPoint
{
    private Vector3[] _traversalVectors;
    private float _radius;

    public WayPoint(Vector3 fromVector3, Vector3 toVector3)
    {
        _traversalVectors = new Vector3[2];
        _traversalVectors[0] = fromVector3;
        _traversalVectors[1] = toVector3;
        _radius = 0f;
    }

    public WayPoint(Vector3 fromVector3, Vector3 offsetVector3, Vector3 toVector3, float radius)
    {
        _traversalVectors = new Vector3[3];
        _traversalVectors[0] = fromVector3;
        _traversalVectors[1] = offsetVector3;
        _traversalVectors[2] = toVector3;
        _radius = radius;
    }

    public Vector3[] TraversalVectors {
        get { return _traversalVectors; }
        set { _traversalVectors = value; }
    }

    public float Radius {
        get { return _radius; }
        set { _radius = value; }
    }

    public override string ToString()
    {
        String output = "TraversalVectors: ";
        foreach (Vector3 traversalVector in _traversalVectors)
        {
            output += traversalVector.ToString() + ", ";
        }
        return output;
    }
}

public interface IPathFindingAlgorithm
{
    #region Methods
    Path FindPath(PathFindingNode startNode, PathFindingNode endNode);
    #endregion
}