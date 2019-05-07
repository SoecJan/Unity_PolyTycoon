using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPathFinder : AbstractPathFinder
{
	#region Methods
	void Start()
	{
		_pathFindingAlgorithm = new NetworkAStarPathFinding();
	}
	#endregion
}

class NetworkNode : Node, IHeapItem<NetworkNode>
{
	#region Attributes
	private NetworkNode _parent;
	private PathFindingNode _pathFindingNode;
	#endregion

	#region Constructors
	public NetworkNode(PathFindingNode pathFindingNode) : base(0,0)
	{
		_pathFindingNode = pathFindingNode;
	}

	public NetworkNode(PathFindingNode pathFindingNode, int hCost, int gCost) : base (hCost, gCost)
	{
		_pathFindingNode = pathFindingNode;
	}
	#endregion

	#region Getter & Setter
	public PathFindingNode PathFindingNode {
		get {
			return _pathFindingNode;
		}

		set {
			_pathFindingNode = value;
		}
	}

	internal new NetworkNode Parent {
		get {
			return _parent;
		}

		set {
			_parent = value;
		}
	}

	public override bool Equals(object obj)
	{
		NetworkNode other = obj as NetworkNode;
		return other != null && PathFindingNode.Equals(other.PathFindingNode);
	}

	#endregion

	public int CompareTo(NetworkNode other)
	{
		return base.CompareTo(other);
	}
}

public class NetworkAStarPathFinding : IPathFindingAlgorithm
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
//					GameObject obj = new GameObject(neighbor.transform.position.ToString());
//					obj.transform.position = neighbor.transform.position + Vector3.up;
					neighborNode = new NetworkNode(neighbor, updatedHCost, updatedGCost) { Parent = currentNetworkNode };
					openSet.Add(neighborNode);
				}
				openSet.Sort();
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
	WayPoint CalculateTraversalVectors(NetworkNode lastNetworkNode, NetworkNode currentNetworkNode)
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

	int DirectionVectorToInt(Vector3 normalizedDirection)
	{
//		Debug.Log(normalizedDirection);
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