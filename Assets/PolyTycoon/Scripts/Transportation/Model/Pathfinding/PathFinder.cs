using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPathFinder
{
    TransportRoute FindPath(TransportRoute transportRoute);
}

public class PathFinder : IPathFinder
{
    private Dictionary<Vehicle.PathType, AbstractPathFindingAlgorithm> _pathFindingAlgorithms;

    public PathFinder(TerrainGenerator terrainGenerator)
    {
        _pathFindingAlgorithms = new Dictionary<Vehicle.PathType, AbstractPathFindingAlgorithm>
        {
            {Vehicle.PathType.Road, new NetworkAStarPathFinding()},
            {Vehicle.PathType.Rail, new RailAStarPathFinding()},
            {
                Vehicle.PathType.Water,
                new TileAStarPathFinding(terrainGenerator, TerrainGenerator.TerrainType.Ocean)
            },
            {Vehicle.PathType.Air, new AirPathFinding()}
        };
    }

    public TransportRoute FindPath(TransportRoute transportRoute)
    {
        AbstractPathFindingAlgorithm _pathFinder = _pathFindingAlgorithms[transportRoute.Vehicle.MoveType];
        foreach (TransportRouteElement transportRouteElement in transportRoute.TransportRouteElements)
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
        return transportRoute;
    }
}