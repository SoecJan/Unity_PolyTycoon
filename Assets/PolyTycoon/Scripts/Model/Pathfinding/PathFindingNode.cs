using System;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public interface IPathFindingNode
{
    PathFindingNode[] NeighborNodes { get; }
    bool IsTraversable();
}

/// <summary>
/// All objects that are supposed to be reached by a vehicle need to have this component.
/// After successful registration at BuildingManager <see cref="BuildingManager"/> this component searches for connected Node, using <see cref="PathFindingNode.OnPlacement"/> in adjacent tiles.
/// <see cref="PathFindingNode.OnDestroy"/> handles the cleanup after a street is destroyed.
/// </summary>
public abstract class PathFindingNode : SimpleMapPlaceable, IPathFindingNode
{
    #region Attributes

    private const int
        NeighborCount = 4; // The amount of streets that can be connected to this one. 4 = Grid, 8 = Diagonal

    // Indices in the neighborStreets Array
    public const int Up = 0;
    public const int Right = 1;
    public const int Down = 2;
    public const int Left = 3;

    private List<ScheduledMover> _upScheduledMovers = null;
    private List<ScheduledMover> _rightScheduledMovers = null;
    private List<ScheduledMover> _downscheduledMovers = null;
    private List<ScheduledMover> _leftscheduledMovers = null;

    private PathFindingNode[] neighborNodes; // Array that holds the reference to the next reachable Node.

    #endregion

    #region Getter & Setter

    private static IBuildingManager BuildingManager { get; set; }

    public PathFindingNode[] NeighborNodes
    {
        get => neighborNodes;

        private set => neighborNodes = value;
    }

    public static int TotalNodeCount { get; private set; }

    protected virtual Vector3 TraversalOffset { get; set; }

    public abstract bool IsTraversable();

    protected abstract bool IsNode(); // Returns true if this object is supposed to be a node for path finding

    protected virtual PathFindingNode AdjacentNodes(int i)
    {
        Vector3 position = gameObject.transform.position + UsedCoordinates[0].UsedCoordinate;
        switch (i)
        {
            case 0:
                position += Vector3.forward;
                break;
            case 1:
                position += Vector3.right;
                break;
            case 2:
                position += Vector3.back;
                break;
            case 3:
                position += Vector3.left;
                break;
        }

        if (BuildingManager != null)
        {
            SimpleMapPlaceable simpleMapPlaceable = BuildingManager.GetNode(position);
            if (simpleMapPlaceable && (simpleMapPlaceable.transform.position + simpleMapPlaceable.UsedCoordinates[0].UsedCoordinate).Equals(position))
            {
                return (PathFindingNode) simpleMapPlaceable;
            }
        }
        return null;
    }

    #endregion

    #region Default Methods

    protected override void Initialize()
    {
        if (BuildingManager == null) BuildingManager = FindObjectOfType<GameHandler>().BuildingManager;
    }

    protected void OnDrawGizmos()
    {
        Vector3 position;
        foreach (NeededSpace coordinate in UsedCoordinates)
        {
            position = gameObject.transform.position + coordinate.UsedCoordinate;
            Color coordinateGizmoColor;
            switch (coordinate.TerrainType)
            {
                case TerrainGenerator.TerrainType.Ocean:
                    coordinateGizmoColor = Color.blue;
                    break;
                case TerrainGenerator.TerrainType.Coast:
                    coordinateGizmoColor = Color.magenta;
                    break;
                case TerrainGenerator.TerrainType.Flatland:
                    coordinateGizmoColor = Color.green;
                    break;
                case TerrainGenerator.TerrainType.Hill:
                    coordinateGizmoColor = Color.gray;
                    break;
                case TerrainGenerator.TerrainType.Mountain:
                    coordinateGizmoColor = Color.yellow;
                    break;
                default:
                    coordinateGizmoColor = Color.black;
                    break;
            }

            Gizmos.color = coordinateGizmoColor;
            Gizmos.DrawSphere(position, 0.1f);
            
            Gizmos.color = Color.blue;

            if (_upScheduledMovers != null)
            {
                Vector3 upPosition = (Vector3.forward/2f) + transform.position;
                for (int i = 0; i < _upScheduledMovers.Count; i++)
                {
                    Gizmos.DrawSphere(upPosition + (i*(Vector3.up/2f)), 0.1f);
                }
            }

            if (_rightScheduledMovers != null)
            {
                Vector3 rightPosition = (Vector3.right/2f) + transform.position;
                for (int i = 0; i < _rightScheduledMovers.Count; i++)
                {
                    Gizmos.DrawSphere(rightPosition + (i*(Vector3.up/2f)), 0.1f);
                }
            }

            if (_downscheduledMovers != null)
            {
                Vector3 downPosition = (Vector3.back/2f) + transform.position;
                for (int i = 0; i < _downscheduledMovers.Count; i++)
                {
                    Gizmos.DrawSphere(downPosition + (i*(Vector3.up/2f)), 0.1f);
                }
            }

            if (_leftscheduledMovers != null)
            {
                Vector3 leftPosition = (Vector3.left/2f) + transform.position;
                for (int i = 0; i < _leftscheduledMovers.Count; i++)
                {
                    Gizmos.DrawSphere(leftPosition + (i*(Vector3.up/2f)), 0.1f);
                }
            }
        }

        position = gameObject.transform.position + UsedCoordinates[0].UsedCoordinate + (Vector3.up / 2);
        Gizmos.color = IsNode() ? Color.yellow : Color.red;
        Gizmos.DrawSphere(position, 0.1f);
    }

    /// <summary>
    /// Cleans up on this object after it has been destroyed
    /// </summary>
    protected void OnDestroy()
    {
        // Remove this instance from the neighbors
        if (!IsPlaced) return;
        TotalNodeCount -= 1;
        for (int i = 0; i < NeighborCount; i++)
        {
            if (neighborNodes[i] == null) continue;
            neighborNodes[i].NeighborNodes[(i + 2) % NeighborCount] = neighborNodes[(i + 2) % NeighborCount];
        }
    }

    /// <summary>
    /// Searches for neighbor streets after being placed on the map
    /// </summary>
    public override void OnPlacement()
    {
        base.OnPlacement();
        if (BuildingManager == null) BuildingManager = FindObjectOfType<GameHandler>().BuildingManager;
        NeighborNodes = new PathFindingNode[NeighborCount];
        TotalNodeCount += 1;
        FindNeighborNodes();
    }

    #endregion

    #region Infrastructure

    public WaypointMoverController RegisterMover(ScheduledMover scheduledMover)
    {
        Debug.Log("Register from: " + scheduledMover.From);
        List<ScheduledMover> list = GetListFromNode(scheduledMover.From == null ? scheduledMover.WaypointMover.MoverTransform.position : scheduledMover.From.ThreadsafePosition);
        list.Add(scheduledMover);
        list.Sort();
        
        int frontMoverIndex = list.IndexOf(scheduledMover) - 1; // Get the transform of the mover in front
        return frontMoverIndex >= 0 ? list[frontMoverIndex].WaypointMover : null;
    }

    private List<ScheduledMover> GetListFromNode(Vector3 previousPosition)
    {
        Vector3Int fromVec3 =
            Vector3Int.RoundToInt((previousPosition - this.ThreadsafePosition).normalized);
        int from = AbstractPathFindingAlgorithm.DirectionVectorToInt(fromVec3);
        
        switch (from)
        {
            case Up:
                return _upScheduledMovers ?? (_upScheduledMovers = new List<ScheduledMover>());
            case Right:
                return _rightScheduledMovers ?? (_rightScheduledMovers = new List<ScheduledMover>());
            case Down:
                return _downscheduledMovers ?? (_downscheduledMovers = new List<ScheduledMover>());
            case Left:
                return _leftscheduledMovers ?? (_leftscheduledMovers = new List<ScheduledMover>());
        }

        return null;
    }

    public void UnregisterMover(WaypointMoverController waypointMover, PathFindingNode previousNode)
    {
        List<ScheduledMover> list = GetListFromNode(previousNode == null ? waypointMover.MoverTransform.position : previousNode.ThreadsafePosition);
        if (list == null) return;
        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].WaypointMover.Equals(waypointMover)) continue;
            Debug.Log("Removed at " + i.ToString());
            list.RemoveAt(i);
            return;
        }
    }
    
    /// <summary>
    /// Finds NeighborNodes in all 4 directions and updates all found Nodes.
    /// </summary>
    private void FindNeighborNodes()
    {
        PathFindingNode[] pathFindingNodes = FindNextNodes();
        foreach (PathFindingNode node in pathFindingNodes)
        {
            if (node)
            {
                node.FindNextNodes();
            }
        }
        if (this is PathFindingConnector connector) connector.UpdateOrientation();
    }

    /// <summary>
    /// Checks all adjacent Nodes in one direction until a Node is found.
    /// </summary>
    /// <returns>The Node that was found in the specified direction</returns>
    private PathFindingNode[] FindNextNodes()
    {
        PathFindingNode[] pathFindingNodes = new PathFindingNode[NeighborCount];
        for (int i = 0; i < NeighborCount; i++)
        {
            PathFindingNode nextNode = AdjacentNodes(i);
            
            while (nextNode && !nextNode.IsNode())
            {
                Array.Clear(nextNode.NeighborNodes, 0, NeighborCount); // Clear Neighbors of non nodes
                nextNode = nextNode.AdjacentNodes(i);
            }

            if (nextNode != this) // Nodes would otherwise add themselves
            {
                pathFindingNodes[i] = nextNode;
            }

            if (pathFindingNodes[i] && IsNode()) // Check if the Node exists and i am a node
            {
                pathFindingNodes[i].NeighborNodes[(i + 2) % NeighborCount] = this;
                if (nextNode is PathFindingConnector connector) connector.UpdateOrientation();
            }
        }

        if (IsNode()) // If i am a node > Safe
        {
            NeighborNodes = pathFindingNodes;
        }

        return pathFindingNodes;
    }

    #endregion

    public abstract WayPoint GetTraversalVectors(int fromDirection, int toDirection);
}

public class ScheduledMover : IComparable<ScheduledMover>
{
    private WaypointMoverController _waypointMover;
    private Vector3 _intersectionVec3;
    private PathFindingNode _from;
    private PathFindingNode _to;

    public ScheduledMover(WaypointMoverController waypointMover, Vector3 intersectionVec3, PathFindingNode from, PathFindingNode to)
    {
        _waypointMover = waypointMover;
        _intersectionVec3 = intersectionVec3;
        _from = from;
        _to = to;
    }

    public WaypointMoverController WaypointMover
    {
        get => _waypointMover;
        set => _waypointMover = value;
    }

    public PathFindingNode From
    {
        get => _from;
        set => _from = value;
    }

    public PathFindingNode To
    {
        get => _to;
        set => _to = value;
    }

    public int CompareTo(ScheduledMover other)
    {
        float myDistance = (this.WaypointMover.MoverTransform.position - this._intersectionVec3).magnitude;
        float otherDistance = (other.WaypointMover.MoverTransform.position - this._intersectionVec3).magnitude;
        return myDistance > otherDistance ? 1 : myDistance < otherDistance ? -1 : 0;
    }
}