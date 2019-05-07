using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailPathFinder : AbstractPathFinder
{
	void Start()
	{
		_pathFindingAlgorithm = new RailAStarPathFinding();
	}
}

public class RailAStarPathFinding : IPathFindingAlgorithm
{
	#region Getter & Setter
	int GetDistance(PathFindingNode nodeA, PathFindingNode nodeB)
	{
		return (int)Math.Abs((nodeA.ThreadsafePosition - nodeB.ThreadsafePosition).magnitude);
	}
	#endregion

	#region Methods
	public Path FindPath(PathFindingNode startNode, PathFindingNode endNode)
	{
		Heap<NetworkNode> openSet = new Heap<NetworkNode>(PathFindingNode.TotalNodeCount);
		HashSet<PathFindingNode> closedSet = new HashSet<PathFindingNode>();
		openSet.Add(new NetworkNode(startNode));

		while (openSet.Count > 0)
		{
			NetworkNode currentNetworkNode = openSet.RemoveFirst();
			closedSet.Add(currentNetworkNode.PathFindingNode);

			if (currentNetworkNode.PathFindingNode == endNode)
			{
				return RetracePath(startNode, currentNetworkNode);
			}

			foreach (PathFindingNode neighbor in currentNetworkNode.PathFindingNode.NeighborNodes)
			{
				if (!neighbor || (!neighbor.IsTraversable() && neighbor != endNode) || closedSet.Contains(neighbor)) continue;

				int newMovementCostToNeighbor = currentNetworkNode.GCost + GetDistance(currentNetworkNode.PathFindingNode, neighbor);
				NetworkNode neighborNetworkNode = new NetworkNode(neighbor, GetDistance(neighbor, endNode), newMovementCostToNeighbor) { Parent = currentNetworkNode };
				if (!openSet.Contains(neighborNetworkNode))
				{
					openSet.Add(neighborNetworkNode);
				}
				else
				{
					openSet.UpdateItem(neighborNetworkNode);
				}
			}
		}
		return null;
	}

	Path RetracePath(PathFindingNode fromNode, NetworkNode toNetworkNode)
	{
		Path path = new Path();

		NetworkNode lastNetworkNode = null;
		NetworkNode currentNetworkNode = toNetworkNode;

		while (currentNetworkNode.PathFindingNode != fromNode)
		{
			if (!(currentNetworkNode.PathFindingNode is Trainstation))
				path.WayPoints.Add(CalculateTraversalVectors(lastNetworkNode, currentNetworkNode));
			lastNetworkNode = currentNetworkNode;
			currentNetworkNode = currentNetworkNode.Parent;
		}
//		path.WayPoints.Add(CalculateTraversalVectors(lastNetworkNode, currentNetworkNode));
		path.WayPoints.Reverse();
		Debug.Log(path.ToString());
		return path;
	}

	/// <summary>
	/// Returns the Traversal Vectors as a WayPoint Object from a given set of PathfindingNodes
	/// </summary>
	/// <param name="lastNetworkNode"></param>
	/// <param name="currentNetworkNode"></param>
	/// <returns></returns>
	WayPoint CalculateTraversalVectors(NetworkNode lastNetworkNode, NetworkNode currentNetworkNode)
	{
		NetworkNode nextNetworkNode = currentNetworkNode.Parent;

		Vector3Int fromVector3 = new Vector3Int();
		if (lastNetworkNode != null)
		{
			fromVector3 = Vector3Int.RoundToInt((lastNetworkNode.PathFindingNode.transform.position - currentNetworkNode.PathFindingNode.transform.position).normalized);
		}

		Vector3Int toVector3 = new Vector3Int();
		if (nextNetworkNode != null)
		{
			toVector3 = Vector3Int.RoundToInt((nextNetworkNode.PathFindingNode.transform.position - currentNetworkNode.PathFindingNode.transform.position).normalized);
		}
		
		

		int fromDirection = DirectionVectorToInt(fromVector3);
		int toDirection = DirectionVectorToInt(toVector3);
		
		Debug.Log("FROM: (" + fromVector3.x + ";" + fromVector3.y + ";" + fromVector3.z + ") " + fromDirection +"; TO: (" + toVector3.x + ";" + toVector3.y + ";" + toVector3.z + ") " + toDirection);

		//Debug.Log(fromVector3.ToString() + ": " + fromDirection + ", " + toVector3.ToString() + ": " + toDirection)
		return currentNetworkNode.PathFindingNode.GetTraversalVectors(toDirection, fromDirection);
	}

	int DirectionVectorToInt(Vector3 normalizedDirection)
	{
		if (normalizedDirection.Equals(Vector3.forward))
		{
			return PathFindingNode.Up;
		}
		else if (normalizedDirection.Equals(Vector3.right))
		{
			return PathFindingNode.Right;
		}
		else if (normalizedDirection.Equals(Vector3.back))
		{
			return PathFindingNode.Down;
		}
		else if (normalizedDirection.Equals(Vector3.left))
		{
			return PathFindingNode.Left;
		}
		return -1;
	}
	#endregion
}