using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainstation : PathFindingTarget, IPathNode
{
    [SerializeField] private Rail _accessRail;
    private Dictionary<PathFindingNode, Path> _paths;
    private Vector3 _traversalDirectionOffsetVector3;

    public Rail AccessRail => _accessRail;

    protected override void Initialize()
    {
        _isClickable = true;
        _paths = new Dictionary<PathFindingNode, Path>();
        _traversalDirectionOffsetVector3 = Vector3.forward;
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