using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPathFinder : AbstractPathFinder
{
    // Start is called before the first frame update
    void Start()
    {
        _pathFindingAlgorithm = new AirAStarPathFinding();
    }
}

class AirAStarPathFinding : IPathFindingAlgorithm
{
    int GetDistance(Vector2 nodeA, Vector2 nodeB)
    {
        return (int) Math.Abs((nodeA - nodeB).magnitude);
    }

    public Path FindPath(PathFindingNode startNode, PathFindingNode endNode)
    {
        Airport fromAirport = startNode as Airport;
        Airport toAirport = endNode as Airport;
        Path path = new Path();

        if (fromAirport && toAirport)
        {
            foreach (WayPoint planeTraversalVector in fromAirport.GetPlaneTraversalVectors(Airport.TAKEOFF))
            {
                path.WayPoints.Add(planeTraversalVector);
            }
            foreach (WayPoint planeTraversalVector in toAirport.GetPlaneTraversalVectors(Airport.LANDING))
            {
                path.WayPoints.Add(planeTraversalVector);
            }
//            Debug.Log(path.ToString());
        }
        else
        {
            Debug.LogError("Not all way points are Airports");
            return null;
        }

        return path;
    }
}
