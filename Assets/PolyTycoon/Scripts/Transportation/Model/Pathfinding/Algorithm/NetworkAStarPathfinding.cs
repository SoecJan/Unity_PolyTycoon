using System.Collections.Generic;
using UnityEngine;

public class NetworkAStarPathFinding : AbstractPathFindingAlgorithm
{
	#region Methods
	public override Path FindPath(PathFindingNode startNode, PathFindingNode endNode)
	{
		List<NetworkNode> openSet = new List<NetworkNode>(PathFindingNode.TotalNodeCount);
		HashSet<PathFindingNode> closedSet = new HashSet<PathFindingNode>();
		openSet.Add(new NetworkNode(startNode));

		while (openSet.Count > 0)
		{
			NetworkNode currentNetworkNode = openSet[0];
			openSet.RemoveAt(0);
			closedSet.Add(currentNetworkNode.PathFindingNode);

			foreach (PathFindingNode neighbor in currentNetworkNode.PathFindingNode.NeighborNodes)
			{
				if (!neighbor) continue;
				if (neighbor.Equals(endNode))
				{
					int endNetworkNodeGCost = currentNetworkNode.GCost + GetDistance(currentNetworkNode.PathFindingNode, neighbor);
					int endNetworkNodeHCost = GetDistance(neighbor, endNode);
					NetworkNode endNetworkNode = new NetworkNode(neighbor, endNetworkNodeHCost, endNetworkNodeGCost) { Parent = currentNetworkNode };
					return RetracePath(startNode, endNetworkNode);
				}
				
				bool isWalkable = neighbor.IsTraversable();
				bool isInClosedSet = closedSet.Contains(neighbor);
				if (!isWalkable || isInClosedSet) continue;

				int updatedGCost = currentNetworkNode.GCost + GetDistance(currentNetworkNode.PathFindingNode, neighbor);
				int updatedHCost = GetDistance(neighbor, endNode);

				NetworkNode neighborNode = openSet.Find(x => x.PathFindingNode.Equals(neighbor));

				if (neighborNode != null)
				{
					if (updatedGCost < neighborNode.GCost)
					{
						neighborNode.GCost = updatedGCost;
						neighborNode.HCost = updatedHCost;
						neighborNode.Parent = currentNetworkNode;
					}
				}
				else
				{
					neighborNode = new NetworkNode(neighbor, updatedHCost, updatedGCost) { Parent = currentNetworkNode };
					openSet.Add(neighborNode);
				}
				openSet.Sort();
			}
		}
		return null;
	}
	
	protected virtual Path RetracePath(PathFindingNode fromNode, NetworkNode toNetworkNode)
	{
		Path path = new Path();
		NetworkNode lastNetworkNode = null;
		NetworkNode currentNetworkNode = toNetworkNode;

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
	/// Returns the Traversal Vectors as a WayPoint Object from a given set of PathfindingNodes
	/// </summary>
	/// <param name="lastNetworkNode"></param>
	/// <param name="currentNetworkNode"></param>
	/// <returns></returns>
	protected WayPoint CalculateTraversalVectors(NetworkNode lastNetworkNode, NetworkNode currentNetworkNode)
	{
		NetworkNode nextNetworkNode = currentNetworkNode.Parent;

		Vector3Int fromVector3 = new Vector3Int();
		if (lastNetworkNode != null)
		{
			fromVector3 = Vector3Int.RoundToInt((lastNetworkNode.PathFindingNode.ThreadsafePosition - currentNetworkNode.PathFindingNode.ThreadsafePosition).normalized);
		}

		Vector3Int toVector3 = new Vector3Int();
		if (nextNetworkNode != null)
		{
			toVector3 = Vector3Int.RoundToInt((nextNetworkNode.PathFindingNode.ThreadsafePosition - currentNetworkNode.PathFindingNode.ThreadsafePosition).normalized);
		}

		int fromDirection = DirectionVectorToInt(fromVector3);
		int toDirection = DirectionVectorToInt(toVector3);

		Debug.Log(fromVector3.ToString() + ": " + fromDirection + ", " + toVector3.ToString() + ": " + toDirection);
		return currentNetworkNode.PathFindingNode.GetTraversalVectors(toDirection, fromDirection);
	}
	#endregion
}