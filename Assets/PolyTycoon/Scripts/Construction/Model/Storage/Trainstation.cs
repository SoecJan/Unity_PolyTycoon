using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainstation : AbstractStorageContainer, IPathNode
{
    [SerializeField] private Rail _accessRail;
    private Dictionary<PathFindingNode, Path> _paths;
    private Vector3 _traversalDirectionOffsetVector3;
    private Vector3 _traversalOffset;

    public Rail AccessRail
    {
        get { return _accessRail; }
    }

    protected override void Initialize()
    {
        base.Initialize();
        IsClickable = true;
        _paths = new Dictionary<PathFindingNode, Path>();
        _traversalDirectionOffsetVector3 = Vector3.forward;
        
    }

    protected override Vector3 TraversalOffset
    {
        get { return _traversalOffset + _traversalDirectionOffsetVector3; }
        set { _traversalOffset = new Vector3(value.x, 0.74f, value.z); }
    }
    
    public override void Rotate(Vector3 axis, float rotationAmount)
    {
        base.Rotate(axis, rotationAmount);
        _traversalDirectionOffsetVector3 =  Quaternion.Euler(0, rotationAmount, 0) * _traversalDirectionOffsetVector3;
    }
    

    public override bool IsTraversable()
    {
        return false;
    }

    public override bool IsNode()
    {
        return true;
    }

    public Path PathTo(PathFindingNode targetNode)
    {
        return _paths.ContainsKey(targetNode) ? _paths[targetNode] : null;
    }

    public void AddPath(PathFindingNode targetNode, Path path)
    {
        if (_paths.ContainsKey(targetNode))
        {
            _paths[targetNode] = path;
        }
        else
        {
            _paths.Add(targetNode, path);
        }
    }

    public void RemovePath(PathFindingNode targetNode)
    {
        _paths.Remove(targetNode);
    }
}