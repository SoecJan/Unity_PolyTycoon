using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class calculates Paths form one <see cref="NetworkNode"/> to another <see cref="NetworkNode"/>.
/// </summary>
public class NetworkAStarPathFinding : AbstractPathFindingAlgorithm
{
	#region Methods
	/// <summary>
	/// Calculates the shortest Path from the startNode to the endNode.
	/// </summary>
	/// <param name="startNode">The node this path is supposed to start on</param>
	/// <param name="endNode">The node this path is supposed to end on</param>
	/// <returns></returns>
	public override Path FindPath(PathFindingNode startNode, PathFindingNode endNode)
	{
		List<NetworkNode> openSet = new List<NetworkNode>(); // List of nodes that need to be checked
		HashSet<PathFindingNode> closedSet = new HashSet<PathFindingNode>(); // List of nodes that have been visited
		openSet.Add(new NetworkNode(startNode)); // Add the first entry

		while (openSet.Count > 0)
		{
			// Move entry to closedSet
			NetworkNode currentNetworkNode = openSet[0];
			openSet.RemoveAt(0);
			closedSet.Add(currentNetworkNode.PathFindingNode);

			// Loop over all neighbors
			foreach (PathFindingNode neighbor in currentNetworkNode.PathFindingNode.NeighborNodes)
			{
				if (!neighbor) continue;
				// Check if the end of this path is in the neighborhood
				if (neighbor.Equals(endNode))
				{
					int endNetworkNodeGCost = currentNetworkNode.GCost + GetDistance(currentNetworkNode.PathFindingNode, neighbor);
					int endNetworkNodeHCost = GetDistance(neighbor, endNode);
					NetworkNode endNetworkNode = new NetworkNode(neighbor, endNetworkNodeHCost, endNetworkNodeGCost) { Parent = currentNetworkNode };
					return RetracePath(startNode, endNetworkNode);
				}
				
				// Check if the current neighbor is supposed to be visited
				bool isWalkable = neighbor.IsTraversable();
				bool isInClosedSet = closedSet.Contains(neighbor);
				if (!isWalkable || isInClosedSet) continue; // Continue if it is not supposed to be visited

				// Calculate heuristics
				int gCost = currentNetworkNode.GCost + GetDistance(currentNetworkNode.PathFindingNode, neighbor);
				int hCost = GetDistance(neighbor, endNode);

				// Update heuristics values on the neighbor or create a new instance
				NetworkNode neighborNode = openSet.Find(x => x.PathFindingNode.Equals(neighbor));
				if (neighborNode != null)
				{
					if (gCost < neighborNode.GCost)
					{
						neighborNode.GCost = gCost;
						neighborNode.HCost = hCost;
						neighborNode.Parent = currentNetworkNode;
					}
				}
				else
				{
					neighborNode = new NetworkNode(neighbor, hCost, gCost) { Parent = currentNetworkNode };
					openSet.Add(neighborNode);
				}
				openSet.Sort(); // Sort ascending on fCost
			}
		}
		return null; // There is no path from startNode to endNode
	}
	
	/// <summary>
	/// Extracts the actual <see cref="Path"/> from the found <see cref="NetworkNode"/>.
	/// </summary>
	/// <param name="fromNode">The node this path was calculated from</param>
	/// <param name="toNetworkNode">The node this path was calculated to</param>
	/// <returns>The path retraced from the nodes</returns>
	protected virtual Path RetracePath(PathFindingNode fromNode, NetworkNode toNetworkNode)
	{
		Path path = new Path();
		NetworkNode lastNetworkNode = null;
		NetworkNode currentNetworkNode = toNetworkNode;
		
		// Actual extraction of the path from the NetworkNodes
		while (currentNetworkNode.PathFindingNode != fromNode)
		{
			path.WayPoints.Add(CalculateTraversalVectors(lastNetworkNode, currentNetworkNode));
			lastNetworkNode = currentNetworkNode;
			currentNetworkNode = currentNetworkNode.Parent;
		}
		path.WayPoints.Add(CalculateTraversalVectors(lastNetworkNode, currentNetworkNode));
		path.WayPoints.Reverse();
		return path;
	}
	
	/// <summary>
	/// Returns the Traversal Vectors as a WayPoint Object from a given set of PathfindingNodes.
	/// These are used to make vehicles like trucks move on the right side of the road and make smooth turns.
	/// </summary>
	/// <param name="lastNetworkNode">The Node the mover was on before</param>
	/// <param name="currentNetworkNode">The Node the mover is currently on</param>
	/// <returns>The Traversal Vectors that need to be taken</returns>
	protected WayPoint CalculateTraversalVectors(NetworkNode lastNetworkNode, NetworkNode currentNetworkNode)
	{
		NetworkNode nextNetworkNode = currentNetworkNode.Parent; // The next node to be visited

		Vector3Int fromVector3 = new Vector3Int(); // The direction the mover came from
		if (lastNetworkNode != null)
		{
			fromVector3 = Vector3Int.RoundToInt((lastNetworkNode.PathFindingNode.ThreadsafePosition - currentNetworkNode.PathFindingNode.ThreadsafePosition).normalized);
		}

		Vector3Int toVector3 = new Vector3Int(); // The direction the mover is moving to
		if (nextNetworkNode != null)
		{
			toVector3 = Vector3Int.RoundToInt((nextNetworkNode.PathFindingNode.ThreadsafePosition - currentNetworkNode.PathFindingNode.ThreadsafePosition).normalized);
		}

		// transform direction to a single integer value representing the direction
		int fromDirection = Util.DirectionVectorToInt(fromVector3);
		int toDirection = Util.DirectionVectorToInt(toVector3);
		
		WayPoint wayPoint = currentNetworkNode.PathFindingNode.GetTraversalVectors(toDirection, fromDirection);
		wayPoint.Node = currentNetworkNode.PathFindingNode;
		return wayPoint;
	}
	#endregion
}