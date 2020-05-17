
using System.Collections.Generic;

/// <summary>
/// This interface describes functionality for a PathFinder
/// </summary>
public interface IPathFinder
{
    /// <summary>
    /// Finds Paths for the given <see cref="TransportRouteElement"/> based on the given <see cref="TransportVehicleData"/>
    /// </summary>
    /// <param name="transportVehicleData">The vehicle that is going to use the path</param>
    /// <param name="transportRouteElements">The route that needs to be calculated</param>
    /// <returns>The populated List of TransportRouteElements</returns>
    List<TransportRouteElement> FindPath(TransportVehicleData transportVehicleData,
        List<TransportRouteElement> transportRouteElements);
}

/// <summary>
/// Wraps the Pathfinding process for multiple <see cref="AbstractPathFindingAlgorithm"/> instances.
/// Determines the <see cref="PathType"/> and therefore this used <see cref="AbstractPathFindingAlgorithm"/> of a
/// given <see cref="TransportVehicleData"/> for Pathfinding.
/// </summary>
public class PathFinder : IPathFinder
{
    private Dictionary<PathType, AbstractPathFindingAlgorithm> _pathFindingAlgorithms;

    public PathFinder(ITerrainGenerator terrainGenerator)
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