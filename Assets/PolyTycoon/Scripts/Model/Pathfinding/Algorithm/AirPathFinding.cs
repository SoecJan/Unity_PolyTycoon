using UnityEngine;

public class AirPathFinding : AbstractPathFindingAlgorithm
{
    public override Path FindPath(PathFindingNode startNode, PathFindingNode endNode)
    {
        Airport fromAirport = startNode.GetComponent<Airport>();
        Airport toAirport = endNode.GetComponent<Airport>();
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
        }
        else
        {
            Debug.LogError("Not all way points are Airports");
            return null;
        }
        return path;
    }
}
