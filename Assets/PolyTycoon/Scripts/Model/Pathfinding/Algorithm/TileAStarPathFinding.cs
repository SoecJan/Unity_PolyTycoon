using System;
using System.Collections.Generic;
using UnityEngine;

public class TileAStarPathFinding : AbstractPathFindingAlgorithm
{
    private TerrainGenerator.TerrainType _terrainType;
    private TerrainGenerator _terrainGenerator;

    public TileAStarPathFinding(TerrainGenerator terrainGenerator, TerrainGenerator.TerrainType terrainType)
    {
        _terrainGenerator = terrainGenerator;
        TerrainType = terrainType;
    }

    private TerrainGenerator.TerrainType TerrainType
    {
        get => _terrainType;
        set => _terrainType = value;
    }

    int GetDistance(Vector2 nodeA, Vector2 nodeB)
    {
        return Mathf.RoundToInt((nodeA - nodeB).magnitude);
    }

    private Vector2Int GetWaterPosition(PathFindingNode simpleMapPlaceable)
    {
        foreach (NeededSpace neededSpace in simpleMapPlaceable.UsedCoordinates)
        {
            if (neededSpace.TerrainType == _terrainType)
            {
                Vector3 waterPosition = neededSpace.UsedCoordinate + simpleMapPlaceable.ThreadsafePosition;
                return new Vector2Int((int)waterPosition.x, (int)waterPosition.z);
            }
        }
        throw new NotSupportedException("No NeededSpace with the " + _terrainType + " terrain type found in " + simpleMapPlaceable.name);
    }

    public override Path FindPath(PathFindingNode startNode, PathFindingNode endNode)
    {
        List<TileNode> openSet = new List<TileNode>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        Vector2Int startPositionVec2 = GetWaterPosition(startNode);
        TileNode startTileNode = new TileNode(startPositionVec2);
        openSet.Add(startTileNode);
        Vector2Int endPosition = GetWaterPosition(endNode);
        
        while (openSet.Count > 0)
        {
            TileNode currentNode = openSet[0];
            openSet.RemoveAt(0);
            closedSet.Add(currentNode.PositionVector2);

            if (GetDistance(currentNode.PositionVector2, endPosition) < 1.5f)
            {
                return RetracePath(startTileNode, currentNode);
            }

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

                TileNode neighborNode = openSet.Find(x => x.PositionVector2.Equals(tilePosition));
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
                    neighborNode = new TileNode(currentNode, tilePosition, updatedHCost, updatedGCost);
                    openSet.Add(neighborNode);
                }
                openSet.Sort();
            }
        }
        return null;
    }

    private Path RetracePath(AbstractAStarNode fromNode, AbstractAStarNode toNode)
    {
        Path path = new Path();
        TileNode currentNode = toNode as TileNode;
        Vector3 offsetVector = (Vector3.forward + Vector3.right) / 2;

        Vector3 wayPointPosition;
        while (currentNode != null && !currentNode.Equals(fromNode as TileNode))
        {
            wayPointPosition = new Vector3(currentNode.PositionVector2.x, 0f, currentNode.PositionVector2.y) + offsetVector;
            path.WayPoints.Add(new WayPoint(wayPointPosition, wayPointPosition));
            currentNode = currentNode.Parent;
        }

        if (currentNode != null)
        {
            wayPointPosition = new Vector3(currentNode.PositionVector2.x, 0f, currentNode.PositionVector2.y) + offsetVector;
            path.WayPoints.Add(new WayPoint(wayPointPosition, wayPointPosition));
        }
        
        path.WayPoints.Reverse();
        return path;
    }
}