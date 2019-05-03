using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

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
        ThreadStart threadStart = delegate { CalculatePath(transportRoute, callback); };
        threadStart.Invoke();
//        StartCoroutine(CalculatePath(transportRoute, callback));
    }

    private void CalculatePath(TransportRoute transportRoute, System.Action<TransportRoute> callback)
    {
        foreach (TransportRouteElement transportRouteElement in transportRoute.TransportRouteElements)
        {
            IPathNode pathNode = transportRouteElement.FromNode as IPathNode;
            Path path;
            if (pathNode != null)
            {
                path = pathNode.PathTo(transportRouteElement.ToNode);
                if (path == null)
                {
                    path = _pathFindingAlgorithm.FindPath(transportRouteElement.FromNode, transportRouteElement.ToNode);
                    pathNode.AddPath(transportRouteElement.ToNode, path);
                }
            }
            else
            {
                path = _pathFindingAlgorithm.FindPath(transportRouteElement.FromNode, transportRouteElement.ToNode);
            }

            transportRouteElement.Path = path;
        }

        callback(transportRoute);
    }
}

class TerrainNode : Node, IHeapItem<TerrainNode>
{
    private TerrainNode _parent;
    private Vector2Int _positionVector2;

    public TerrainNode(Vector2Int position) : base(0, 0)
    {
        _positionVector2 = position;
    }

    public TerrainNode(TerrainNode node, Vector2Int position, int hCost, int gCost) : base(hCost, gCost)
    {
        _parent = node;
        _positionVector2 = position;
    }

    public Vector2Int PositionVector2
    {
        get { return _positionVector2; }
        set { _positionVector2 = value; }
    }

    public TerrainNode Parent
    {
        get { return _parent; }
        set { _parent = value; }
    }

    public override int FCost
    {
        get { return (HCost) + GCost; }
    }

    public int CompareTo(TerrainNode other)
    {
        return -base.CompareTo(other);
    }

    public override bool Equals(object obj)
    {
        TerrainNode other = obj as TerrainNode;
        return other != null && PositionVector2.Equals(other.PositionVector2);
    }

    public override string ToString()
    {
        return PositionVector2.ToString() + ": FCost: " + FCost + ", "; //+ " GCost: " + GCost + " HCost: " + HCost;
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
        return Mathf.RoundToInt((nodeA - nodeB).magnitude);
    }

    private Vector3 GetWaterPosition(SimpleMapPlaceable simpleMapPlaceable)
    {
        foreach (NeededSpace neededSpace in simpleMapPlaceable.UsedCoordinates)
        {
            if (neededSpace.TerrainType == _terrainType)
                return neededSpace.UsedCoordinate + simpleMapPlaceable.transform.position;
        }

        Debug.LogError("No NeededSpace with the " + _terrainType + " terrain type found.");
        return simpleMapPlaceable.transform.position;
    }

    public Path FindPath(PathFindingNode startNode, PathFindingNode endNode)
    {
        List<TerrainNode> openSet = new List<TerrainNode>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        Vector3 startPositionVec3 = GetWaterPosition(startNode);
        Vector2Int startPositionVec2 = new Vector2Int((int) startPositionVec3.x, (int) startPositionVec3.z);
        TerrainNode startTerrainNode = new TerrainNode(startPositionVec2);
        openSet.Add(startTerrainNode);
        Vector3 endPositionVec3 = GetWaterPosition(endNode);
        Vector2Int endPosition = new Vector2Int((int) endPositionVec3.x, (int) endPositionVec3.z);

        Debug.Log("Start: " + startPositionVec3 + "; End: " + endPosition);

        while (openSet.Count > 0)
        {
            TerrainNode currentNode = openSet[0];
            openSet.RemoveAt(0);
            closedSet.Add(currentNode.PositionVector2);

            if (currentNode.PositionVector2.Equals(endPosition))
            {
                return RetracePath(startTerrainNode, currentNode);
            }

            // Check every Tile around the current tile
            for (int i = 0; i < 8; i++)
            {
                Vector2Int tilePosition;

                switch (i)
                {
                    case 0:
                        tilePosition = currentNode.PositionVector2 + Vector2Int.up;
                        break;
                    case 1:
                        tilePosition = currentNode.PositionVector2 + Vector2Int.down;
                        break;
                    case 2:
                        tilePosition = currentNode.PositionVector2 + Vector2Int.left;
                        break;
                    case 3:
                        tilePosition = currentNode.PositionVector2 + Vector2Int.right;
                        break;
                    case 4:
                        tilePosition = currentNode.PositionVector2 + Vector2Int.up + Vector2Int.left;
                        break;
                    case 5:
                        tilePosition = currentNode.PositionVector2 + Vector2Int.up + Vector2Int.right;
                        break;
                    case 6:
                        tilePosition = currentNode.PositionVector2 + Vector2Int.down + Vector2Int.left;
                        break;
                    case 7:
                        tilePosition = currentNode.PositionVector2 + Vector2Int.down + Vector2Int.right;
                        break;
                    default:
                        tilePosition = Vector2Int.zero;
                        Debug.LogError("Should not reach here");
                        break;
                }

                bool isWalkable = _terrainGenerator.IsSuitedTerrain(TerrainType, tilePosition.x+0.5f, tilePosition.y+0.5f);
                
                bool isInClosedSet = closedSet.Contains(tilePosition);
                if (!isWalkable || isInClosedSet) continue;

                int updatedGCost = currentNode.GCost + GetDistance(currentNode.PositionVector2, tilePosition);
                int updatedHCost = GetDistance(tilePosition, endPosition);

                TerrainNode neighborNode = openSet.Find(x => x.PositionVector2.Equals(tilePosition));
                if (neighborNode != null)
                {
                    if (updatedGCost < neighborNode.GCost)
                    {
                        neighborNode.GCost = updatedGCost;
                        neighborNode.HCost = updatedHCost;
                        neighborNode.Parent = currentNode;
                    }
                }
                else
                {
                    neighborNode = new TerrainNode(currentNode, tilePosition, updatedHCost, updatedGCost);
                    openSet.Add(neighborNode);
                }
                openSet.Sort();
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