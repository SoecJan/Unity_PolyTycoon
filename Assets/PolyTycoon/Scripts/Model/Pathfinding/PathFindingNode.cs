using System;
using System.Collections.Generic;
using System.Linq;
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
public abstract class PathFindingNode : MonoBehaviour, IPathFindingNode
{
    #region Attributes

    protected SimpleMapPlaceable _simpleMapPlaceable;
    public const int NeighborCount = 4; // The amount of streets that can be connected to this one. 4 = Grid, 8 = Diagonal

    // Indices in the neighborNodes, _scheduledMovers, _blockingMovers Arrays
    public const int Up = 0;
    public const int Right = 1;
    public const int Down = 2;
    public const int Left = 3;

    private PathFindingNode[] neighborNodes; // Array that holds the reference to the next reachable Node.

    #endregion

    #region Getter & Setter

    public static IBuildingManager BuildingManager { get; set; }

    public PathFindingNode[] NeighborNodes
    {
        get => neighborNodes;

        private set => neighborNodes = value;
    }

    protected List<NeededSpace> UsedCoordinates
    {
        get => _simpleMapPlaceable.UsedCoordinates;
    }

    public Vector3 ThreadsafePosition => _simpleMapPlaceable.ThreadsafePosition;

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
            PathFindingNode pathFindingNode = BuildingManager.GetNode(position);
            if (!pathFindingNode) return null;
            SimpleMapPlaceable simpleMapPlaceable = pathFindingNode.GetComponent<SimpleMapPlaceable>();
            if (simpleMapPlaceable &&
                (simpleMapPlaceable.transform.position + simpleMapPlaceable.UsedCoordinates[0].UsedCoordinate)
                .Equals(position))
            {
                return pathFindingNode;
            }
        }

        return null;
    }

    #endregion

    #region Default Methods

    protected virtual void Awake()
    {
        _simpleMapPlaceable = GetComponent<SimpleMapPlaceable>();
        _simpleMapPlaceable._OnPlacementEvent += OnPlacement;
    }

    public virtual void Start()
    {
        if (BuildingManager == null) BuildingManager = FindObjectOfType<GameHandler>().BuildingManager;
    }

    protected void OnDrawGizmos()
    {
        Vector3 position;
        if (_simpleMapPlaceable == null) _simpleMapPlaceable = GetComponent<SimpleMapPlaceable>();
        if (UsedCoordinates == null) return;
        foreach (NeededSpace coordinate in UsedCoordinates)
        {
            // Visualize used coordinates
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
        if (!_simpleMapPlaceable.IsPlaced) return;
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
    protected virtual void OnPlacement(SimpleMapPlaceable simpleMapPlaceable)
    {
        if (BuildingManager == null) BuildingManager = FindObjectOfType<GameHandler>().BuildingManager;
        NeighborNodes = new PathFindingNode[NeighborCount];
        TotalNodeCount += 1;
        FindNeighborNodes();
    }

    #endregion

    #region Infrastructure

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

    public ScheduledMover(WaypointMoverController waypointMover, Vector3 intersectionVec3, PathFindingNode from,
        PathFindingNode to)
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

/// <summary>
/// Holds information for intersection logic.
/// The arrays are filled with relative index specific masks.
/// Usage: Get the from direction (Up, Right, Down, Left) and add the masked elements of these array.
/// That will give you the indices that need to be blocked on <see cref="PathFindingNode"/> _blockingMovers
/// </summary>
struct TrafficIntersectionMask
{
    public static int[,] straight =
    {
        { -1, 0 },
        { -1, 1 },
        { -1, 2 },
        { -1, 3 },
        // { 0, 0 }, Followers are allowed to move anywhere
        // { 0, 1 },
        // { 0, 2 },
        // { 0, 3 },
        { 1, 0 },
        { 1, 1 },
        { 1, 2 },
        // { 1, 3 }, Allow this one to turn right
        { 2, 0 },
        { 2, 1 },
        // { 2, 2 }, Allow the other side to move straight
        // { 2, 3 }, Allow the other side to turn right
    };

    public static int[,] right =
    {
        { -1, 0 },
        // { -1, 1 },
        // { -1, 2 },
        // { -1, 3 },
        // { 0, 0 },
        // { 0, 1 },
        // { 0, 2 },
        // { 0, 3 },
        { 1, 0 },
        // { 1, 1 },
        // { 1, 2 },
        // { 1, 3 }, 
        { 2, 0 },
        // { 2, 1 },
        // { 2, 2 }, 
        // { 2, 3 }, 
    };

    public static int[,] left =
    {
        { -1, 0 },
        // { -1, 1 },
        { -1, 2 },
        { -1, 3 },
        // { 0, 0 },
        // { 0, 1 },
        // { 0, 2 },
        // { 0, 3 },
        { 1, 0 },
        { 1, 1 },
        { 1, 2 },
        // { 1, 3 }, 
        { 2, 0 },
        { 2, 1 },
        { 2, 2 }, 
        { 2, 3 }, 
    };
}