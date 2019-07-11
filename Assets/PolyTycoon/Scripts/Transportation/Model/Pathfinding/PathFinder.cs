using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private Dictionary<Vehicle.PathType, AbstractPathFindingAlgorithm> _pathFindingAlgorithms;

    private void Start()
    {
        _pathFindingAlgorithms = new Dictionary<Vehicle.PathType, AbstractPathFindingAlgorithm>();
        _pathFindingAlgorithms.Add(Vehicle.PathType.Road, new NetworkAStarPathFinding());
        _pathFindingAlgorithms.Add(Vehicle.PathType.Rail, new RailAStarPathFinding());
        _pathFindingAlgorithms.Add(Vehicle.PathType.Water, new TileAStarPathFinding(FindObjectOfType<TerrainGenerator>(),TerrainGenerator.TerrainType.Ocean));
        _pathFindingAlgorithms.Add(Vehicle.PathType.Air, new AirPathFinding());
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