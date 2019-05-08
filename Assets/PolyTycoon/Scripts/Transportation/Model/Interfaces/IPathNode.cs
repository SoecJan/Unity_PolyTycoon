public interface IPathNode
{
    Path PathTo(PathFindingNode targetNode);

    void AddPath(PathFindingNode targetNode, Path path);

    void RemovePath(PathFindingNode targetNode);
}