using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPathFinder : AbstractPathFinder
{
    private TerrainGenerator _terrainGenerator;

    void Start()
    {
        _terrainGenerator = FindObjectOfType<TerrainGenerator>();
        _pathFindingAlgorithm = new TerrainAStarPathFinding(_terrainGenerator, TerrainGenerator.TerrainType.Ocean);
    }

    public override void FindPath(TransportRoute transportRoute, Action<TransportRoute> callback)
    {
        StartCoroutine(CalculatePath(transportRoute, callback));
    }

    private IEnumerator CalculatePath(TransportRoute transportRoute, System.Action<TransportRoute> callback)
    {
        foreach (TransportRouteElement transportRouteElement in transportRoute.TransportRouteElements)
        {
            Path path = _pathFindingAlgorithm.FindPath(transportRouteElement.FromNode, transportRouteElement.ToNode);
            transportRouteElement.Path = path;
        }

        callback(transportRoute);
        yield return null;
    }
}

class TerrainNode : Node, IHeapItem<TerrainNode>
{
    private TerrainNode _parent;
    private Vector2 _positionVector2;

    public TerrainNode(Vector2 position) : base(0, 0)
    {
        _positionVector2 = position;
    }

    public TerrainNode(TerrainNode node, Vector2 position, int hCost, int gCost) : base(hCost, gCost)
    {
        _parent = node;
        _positionVector2 = position;
    }

    public Vector2 PositionVector2
    {
        get { return _positionVector2; }
        set { _positionVector2 = value; }
    }

    public TerrainNode Parent
    {
        get { return _parent; }
        set { _parent = value; }
    }

    public int CompareTo(TerrainNode other)
    {
        return base.CompareTo(other);
    }
}

class TerrainAStarPathFinding : IPathFindingAlgorithm
{
    private TerrainGenerator.TerrainType _terrainType;
    private TerrainGenerator _terrainGenerator;

    public TerrainAStarPathFinding(TerrainGenerator terrainGenerator, TerrainGenerator.TerrainType terrainType)
    {
        _terrainGenerator = terrainGenerator;
        TerrainType = terrainType;
    }

    public TerrainGenerator.TerrainType TerrainType
    {
        get { return _terrainType; }
        set { _terrainType = value; }
    }

    int GetDistance(Vector2 nodeA, Vector2 nodeB)
    {
        return (int) Math.Abs((nodeA - nodeB).magnitude);
    }

    private Vector3 GetWaterPosition(SimpleMapPlaceable simpleMapPlaceable)
    {
        foreach (NeededSpace neededSpace in simpleMapPlaceable.UsedCoordinates)
        {
            if (neededSpace.TerrainType == _terrainType) 
                return neededSpace.UsedCoordinate + simpleMapPlaceable.transform.position;
        }
           Debug.LogError("No NeededSpace with the " + _terrainType +" terrain type found.");
        return simpleMapPlaceable.transform.position;
    }
    
    public Path FindPath(PathFindingNode startNode, PathFindingNode endNode)
    {
        Heap<TerrainNode> openSet = new Heap<TerrainNode>(1000);
        HashSet<Vector2> closedSet = new HashSet<Vector2>();

        var startPositionVec3 = GetWaterPosition(startNode);
        TerrainNode startTerrainNode = new TerrainNode(new Vector2(startPositionVec3.x, startPositionVec3.z));
        openSet.Add(startTerrainNode);
        var endPositionVec3 = GetWaterPosition(endNode);
        Vector2Int endPosition = Vector2Int.FloorToInt(new Vector2(endPositionVec3.x, endPositionVec3.z));

        Debug.Log("Start: " + startPositionVec3 + "; End: " + endPosition);
        
        while (openSet.Count > 0)
        {
            TerrainNode currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode.PositionVector2);

            Vector2Int currentPositionVec2 = Vector2Int.FloorToInt(currentNode.PositionVector2);
            
            if (currentPositionVec2.Equals(endPosition))
            {
                return RetracePath(startTerrainNode, currentNode);
            }

            // Check every Tile around the current tile
            for (int i = 0; i < 8; i++)
            {
                Vector2 tilePosition;

                switch (i)
                {
                    case 0:
                        tilePosition = currentPositionVec2 + Vector2.up;
                        break;
                    case 1:
                        tilePosition = currentPositionVec2 + Vector2.down;
                        break;
                    case 2:
                        tilePosition = currentPositionVec2 + Vector2.left;
                        break;
                    case 3:
                        tilePosition = currentPositionVec2 + Vector2.right;
                        break;
                    case 4:
                        tilePosition = currentPositionVec2 + Vector2.up + Vector2.left;
                        break;
                    case 5:
                        tilePosition = currentPositionVec2 + Vector2.up + Vector2.right;
                        break;
                    case 6:
                        tilePosition = currentPositionVec2 + Vector2.down + Vector2.left;
                        break;
                    case 7:
                        tilePosition = currentPositionVec2 + Vector2.down + Vector2.right;
                        break;
                    default:
                        tilePosition = Vector2.zero;
                        Debug.LogError("Should not reach here");
                        break;
                }

                bool isTerrainSuited = _terrainGenerator.IsSuitedTerrain(TerrainType, currentPositionVec2.x,  currentPositionVec2.y);
                bool isEndNode = tilePosition == endPosition;
                bool alreadyLookedAt = closedSet.Contains(tilePosition);
                
                
                if ((!isTerrainSuited && !isEndNode) || alreadyLookedAt) continue;
                
                int updatedGCost = currentNode.GCost + GetDistance(currentNode.PositionVector2, tilePosition);
                int updatedHCost = GetDistance(tilePosition, endPosition);
                TerrainNode neighborNode = new TerrainNode(currentNode, tilePosition, updatedHCost, updatedGCost);
                if (!openSet.Contains(neighborNode))
                {
                    openSet.Add(neighborNode);
                }
                else
                {
                    openSet.UpdateItem(neighborNode);
                }
            }
        }

        return null;
    }

    Path RetracePath(TerrainNode fromNode, TerrainNode toNode)
    {
        Path path = new Path();

        TerrainNode lastNode = null;
        TerrainNode currentNode = toNode;
        Vector3 wayPointPosition = Vector3.zero;
        
        while (!currentNode.Equals(fromNode))
        {
            wayPointPosition = new Vector3(currentNode.PositionVector2.x, 0f, currentNode.PositionVector2.y);
            path.WayPoints.Add(new WayPoint(wayPointPosition, wayPointPosition));
            lastNode = currentNode;
            currentNode = currentNode.Parent;
        }

        wayPointPosition = new Vector3(currentNode.PositionVector2.x, 0f, currentNode.PositionVector2.y);
        path.WayPoints.Add(new WayPoint(wayPointPosition, wayPointPosition));
        path.WayPoints.Reverse();
        
        Debug.Log(path);
        
        return path;
    }
}