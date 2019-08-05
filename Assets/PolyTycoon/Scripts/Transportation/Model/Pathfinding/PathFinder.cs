
using System.Collections.Generic;

public interface IPathFinder
{
    List<TransportRouteElement> FindPath(TransportVehicleData transportVehicleData,
        List<TransportRouteElement> transportRouteElements);
}

public class PathFinder : IPathFinder
{
    private Dictionary<PathType, AbstractPathFindingAlgorithm> _pathFindingAlgorithms;

    public PathFinder(TerrainGenerator terrainGenerator)
    {
        _pathFindingAlgorithms = new Dictionary<PathType, AbstractPathFindingAlgorithm>
        {
            {PathType.Road, new NetworkAStarPathFinding()},
            {PathType.Rail, new RailAStarPathFinding()},
            {
                PathType.Water,
                new TileAStarPathFinding(terrainGenerator, TerrainGenerator.TerrainType.Ocean)
            },
            {PathType.Air, new AirPathFinding()}
        };
    }

    public List<TransportRouteElement> FindPath(TransportVehicleData transportVehicleData,
        List<TransportRouteElement> transportRouteElements)
    {
        AbstractPathFindingAlgorithm _pathFinder = _pathFindingAlgorithms[transportVehicleData.PathType];
        foreach (TransportRouteElement transportRouteElement in transportRouteElements)
        {
            IPathNode pathNode = transportRouteElement.FromNode as IPathNode;
            Path path;
            if (pathNode != null)
            {
                path = pathNode.PathTo(transportRouteElement.ToNode);
                if (path == null)
                {
                    path = _pathFinder.FindPath(transportRouteElement.FromNode, transportRouteElement.ToNode);
                    pathNode.AddPath(transportRouteElement.ToNode, path);
                }
            }
            else
            {
                path = _pathFinder.FindPath(transportRouteElement.FromNode, transportRouteElement.ToNode);
            }
            transportRouteElement.Path = path;
        }
        return transportRouteElements;
    }
}