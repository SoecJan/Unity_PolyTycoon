using System.Collections.Generic;

public abstract class AbstractStorageContainer : PathFindingNode, IStore, IPathNode
{
    private Dictionary<ProductData, ProductStorage> _storedProducts;
    private Dictionary<PathFindingNode, Path> _paths;
    
    protected override void Initialize()
    {
        _storedProducts = new Dictionary<ProductData, ProductStorage>();
        _paths = new Dictionary<PathFindingNode, Path>();
        
    }

    public Dictionary<ProductData, ProductStorage> StoredProducts()
    {
        return _storedProducts;
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