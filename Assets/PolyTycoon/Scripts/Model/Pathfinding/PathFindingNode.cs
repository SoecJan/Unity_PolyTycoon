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
public abstract class PathFindingNode : SimpleMapPlaceable, IPathFindingNode
{
    #region Attributes

    private const int
        NeighborCount = 4; // The amount of streets that can be connected to this one. 4 = Grid, 8 = Diagonal

    // Indices in the neighborNodes, _scheduledMovers, _blockingMovers Arrays
    public const int Up = 0;
    public const int Right = 1;
    public const int Down = 2;
    public const int Left = 3;

    private List<ScheduledMover>[] _scheduledMovers; // All incoming movers are registered here
    /// <summary>
    /// Holds information on blocked pathways by Movers that are currently on the intersection.
    /// Movers call <see cref="GetTrafficLightStatus"/> to figure out if this intersection is passable.
    /// The key: Vector2Int(from, to). If it already exists the path is blocked by another mover.
    /// The value: Mover that is blocking the path. Needed to specify if a given mover is blocking it's own path.
    /// </summary>
    private Dictionary<Vector2Int, List<WaypointMoverController>> _blockingMovers;

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
            if (simpleMapPlaceable &&
                (simpleMapPlaceable.transform.position + simpleMapPlaceable.UsedCoordinates[0].UsedCoordinate)
                .Equals(position))
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
        _scheduledMovers = new List<ScheduledMover>[NeighborCount];
        _blockingMovers = new Dictionary<Vector2Int, List<WaypointMoverController>>();
    }

    protected void OnDrawGizmos()
    {
        Vector3 position;
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

            // Visualize registered movers
            Gizmos.color = Color.blue;
            for (int i = 0; i < _scheduledMovers.Length; i++)
            {
                if (_scheduledMovers[i] == null) continue;
                Vector3 offset;
                switch (i)
                {
                    case Up: offset = Vector3.forward;
                        break;
                    case Right: offset = Vector3.right;
                        break;
                    case Down: offset = Vector3.back;
                        break;
                    case Left: offset = Vector3.left;
                        break;
                    default: offset = Vector3.zero;
                        break;
                }
                for (int j = 0; j < _scheduledMovers[i].Count; j++)
                {
                    Gizmos.DrawSphere(offset + (j * (Vector3.up / 2f)), 0.1f);
                }
            }

            // Visualize Traffic Logic
            foreach (List<WaypointMoverController> moverControllers in _blockingMovers.Values)
            {
                foreach (WaypointMoverController moverController in moverControllers)
                {
                    if (moverController.CurrentWaypointIndex < 0 
                        || moverController.CurrentWaypointIndex >= moverController.WaypointList.Count) continue;
                    Gizmos.DrawLine(ThreadsafePosition, moverController.MoverTransform.position);
                    WayPoint wayPoint = moverController.WaypointList[moverController.CurrentWaypointIndex];
                    for (int j = 0; j < wayPoint.TraversalVectors.Length - 1; j++)
                    {
                        Gizmos.DrawLine(wayPoint.TraversalVectors[j], wayPoint.TraversalVectors[j+1]);
                    }
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

    public bool TrafficLightStatus(WaypointMoverController mover, PathFindingNode previousNode,
        PathFindingNode nextNode)
    {
        if (previousNode == null) return true;

        bool horizontal = NeighborNodes[Left] != null && NeighborNodes[Right] != null;
        bool vertical = NeighborNodes[Up] != null && NeighborNodes[Down] != null;

        if (!horizontal && !vertical)
        {
            return true;
        }
        Vector3Int fromVec3 =
            Vector3Int.RoundToInt((previousNode.ThreadsafePosition - this.ThreadsafePosition).normalized);
        int from = AbstractPathFindingAlgorithm.DirectionVectorToInt(fromVec3);
        Vector3Int toVec3 =
            Vector3Int.RoundToInt((this.ThreadsafePosition - nextNode.ThreadsafePosition).normalized);
        int to = AbstractPathFindingAlgorithm.DirectionVectorToInt(toVec3);

        return GetTrafficLightStatus(mover, from, to);
    }

    private bool GetTrafficLightStatus(WaypointMoverController mover, int from, int to)
    {
        // if (from == 0) return false;

        int[,] blockedMask;
        if (from % 2 == to % 2) // Go straight
        {
            blockedMask = TrafficIntersectionMask.straight;
        } 
        else if ((to + 1) % NeighborCount == from) // Go right
        {
            blockedMask = TrafficIntersectionMask.right;
        }
        else if ((from + 1) % NeighborCount == to) // Go left
        {
            blockedMask = TrafficIntersectionMask.left;
        }
        else // Not an intersection
        {
            Debug.LogError("Nothing to block");
            return true;
        }

        // Check if intersection is blocked
        
        Vector2Int neededPath = new Vector2Int(from, to);
        bool blockExists = _blockingMovers.ContainsKey(neededPath);
        bool moverIsTheOnlyBlocker = blockExists && (_blockingMovers[neededPath].Count == 0 
                                                     || (_blockingMovers[neededPath].Count == 1 
                                                         && _blockingMovers[neededPath].Contains(mover)));
        if (blockExists && !moverIsTheOnlyBlocker) return false;

        // Block intersection
        for (int i = 0; i < blockedMask.GetLength(0); i++)
        {
            int blockedFrom = Mod(blockedMask[i, 0] + from, NeighborCount);
            int blockedTo = Mod(blockedMask[i, 1] + to, NeighborCount);
            Vector2Int key = new Vector2Int(blockedFrom, blockedTo);
            if (!_blockingMovers.ContainsKey(key))
            {
                List<WaypointMoverController> list = new List<WaypointMoverController> {mover};
                _blockingMovers.Add(key, list);
            } else if (!_blockingMovers[key].Contains(mover))
            {
                _blockingMovers[key].Add(mover);
            }
        }
        return true;
    }

    public static int Mod(int x, int m) {
        return (x%m + m)%m;
    }

    public void RegisterMover(ScheduledMover scheduledMover)
    {
        // Debug.Log("Register from: " + scheduledMover.From);
        List<ScheduledMover> list = GetListFromNode(scheduledMover.From == null
            ? scheduledMover.WaypointMover.MoverTransform.position
            : scheduledMover.From.ThreadsafePosition);
        list.Add(scheduledMover);
        list.Sort();

        int addedMoverIndex = list.IndexOf(scheduledMover);
        int frontMoverIndex = addedMoverIndex - 1; // Get the transform of the mover in front
        int backMoverIndex = addedMoverIndex + 1;
        if (backMoverIndex < list.Count)
        {
            WaypointMoverController backMover = list[backMoverIndex].WaypointMover;
            backMover.FrontMover = list[addedMoverIndex].WaypointMover;
            scheduledMover.WaypointMover.BackMover = backMover;
        }

        if (frontMoverIndex >= 0)
        {
            WaypointMoverController frontMover = list[frontMoverIndex].WaypointMover;
            frontMover.BackMover = list[addedMoverIndex].WaypointMover;
            scheduledMover.WaypointMover.FrontMover = frontMover;
        }
    }

    private List<ScheduledMover> GetListFromNode(Vector3 previousPosition)
    {
        Vector3Int fromVec3 =
            Vector3Int.RoundToInt((previousPosition - this.ThreadsafePosition).normalized);
        int from = AbstractPathFindingAlgorithm.DirectionVectorToInt(fromVec3);

        return _scheduledMovers[from] ?? (_scheduledMovers[from] = new List<ScheduledMover>());
    }

    public void UnregisterMover(WaypointMoverController waypointMover, PathFindingNode previousNode)
    {
        foreach (KeyValuePair<Vector2Int,List<WaypointMoverController>> keyValuePair in _blockingMovers)
        {
            keyValuePair.Value.Remove(waypointMover);
        }
        List<ScheduledMover> list = GetListFromNode(previousNode == null
            ? waypointMover.MoverTransform.position
            : previousNode.ThreadsafePosition);
        if (list == null) return;
        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].WaypointMover.Equals(waypointMover)) continue;
            // Debug.Log("Removed at " + i.ToString());
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