using System.Collections.Generic;
using UnityEngine;

public class TrafficController : MonoBehaviour
{
    private PathFindingNode _pathFindingNode;

    private List<ScheduledMover>[] _scheduledMovers; // All incoming movers are registered here

    /// <summary>
    /// Holds information on blocked pathways by Movers that are currently on the intersection.
    /// Movers call <see cref="GetTrafficLightStatus"/> to figure out if this intersection is passable.
    /// The key: Vector2Int(from, to). If it already exists the path is blocked by another mover.
    /// The value: Mover that is blocking the path. Needed to specify if a given mover is blocking it's own path.
    /// </summary>
    private Dictionary<Vector2Int, List<WaypointMoverController>> _blockingMovers;

    // Start is called before the first frame update
    void Start()
    {
        _pathFindingNode = GetComponent<PathFindingNode>();
        _scheduledMovers = new List<ScheduledMover>[PathFindingNode.NeighborCount];
        _blockingMovers = new Dictionary<Vector2Int, List<WaypointMoverController>>();
    }

    protected void OnDrawGizmos()
    {
        // Visualize registered movers
        if (_scheduledMovers == null) return;
        Gizmos.color = Color.blue;
        for (int i = 0; i < _scheduledMovers.Length; i++)
        {
            if (_scheduledMovers[i] == null) continue;
            Vector3 offset;
            switch (i)
            {
                case PathFindingNode.Up:
                    offset = Vector3.forward;
                    break;
                case PathFindingNode.Right:
                    offset = Vector3.right;
                    break;
                case PathFindingNode.Down:
                    offset = Vector3.back;
                    break;
                case PathFindingNode.Left:
                    offset = Vector3.left;
                    break;
                default:
                    offset = Vector3.zero;
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
                Gizmos.DrawLine(_pathFindingNode.ThreadsafePosition, moverController.MoverTransform.position);
                WayPoint wayPoint = moverController.WaypointList[moverController.CurrentWaypointIndex];
                for (int j = 0; j < wayPoint.TraversalVectors.Length - 1; j++)
                {
                    Gizmos.DrawLine(wayPoint.TraversalVectors[j], wayPoint.TraversalVectors[j + 1]);
                }
            }
        }
    }
    
    public bool TrafficLightStatus(WaypointMoverController mover, PathFindingNode previousNode,
        PathFindingNode nextNode)
    {
        if (previousNode == null) return true;

        bool horizontal = _pathFindingNode.NeighborNodes[PathFindingNode.Left] != null && _pathFindingNode.NeighborNodes[PathFindingNode.Right] != null;
        bool vertical = _pathFindingNode.NeighborNodes[PathFindingNode.Up] != null && _pathFindingNode.NeighborNodes[PathFindingNode.Down] != null;

        if (!horizontal && !vertical)
        {
            return true;
        }
        Vector3Int fromVec3 =
            Vector3Int.RoundToInt((previousNode.ThreadsafePosition - _pathFindingNode.ThreadsafePosition).normalized);
        int from = Util.DirectionVectorToInt(fromVec3);
        Vector3Int toVec3 =
            Vector3Int.RoundToInt((_pathFindingNode.ThreadsafePosition - nextNode.ThreadsafePosition).normalized);
        int to = Util.DirectionVectorToInt(toVec3);

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
        else if ((to + 1) % PathFindingNode.NeighborCount == from) // Go right
        {
            blockedMask = TrafficIntersectionMask.right;
        }
        else if ((from + 1) % PathFindingNode.NeighborCount == to) // Go left
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
            int blockedFrom = Util.Mod(blockedMask[i, 0] + from, PathFindingNode.NeighborCount);
            int blockedTo = Util.Mod(blockedMask[i, 1] + to, PathFindingNode.NeighborCount);
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
            Vector3Int.RoundToInt((previousPosition - _pathFindingNode.ThreadsafePosition).normalized);
        int from = Util.DirectionVectorToInt(fromVec3);

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
}