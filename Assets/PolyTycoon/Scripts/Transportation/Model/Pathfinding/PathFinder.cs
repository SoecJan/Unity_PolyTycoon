using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PolyTycoon.Scripts.Transportation.Model.TransportRoute;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;

namespace Assets.PolyTycoon.Scripts.Transportation.Model.Pathfinding
{
	public class PathFinder : MonoBehaviour
	{
		#region Attributes
		public System.Action<TransportRoute.TransportRoute> OnPathFound;
		private IPathFindingAlgorithm _pathFindingAlgorithm;
		#endregion

		#region Methods
		void Start()
		{
			_pathFindingAlgorithm = new AStarPathFinding();
		}

		public void FindPath(TransportRoute.TransportRoute transportRoute)
		{
			StartCoroutine(CalculatePath(transportRoute));
		}

		/// <summary>
		/// Searches for a path between given points. IF the <see cref="Path.Nodes"/> List is empty or null there is no path between given points.
		/// </summary>
		/// <param name="startNode"></param>
		/// <param name="endNode"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		IEnumerator CalculatePath(TransportRoute.TransportRoute transportRoute)
		{
			foreach (TransportRouteElement transportRouteElement in transportRoute.TransportRouteElements)
			{
				Path path = _pathFindingAlgorithm.FindPath(transportRouteElement.FromNode, transportRouteElement.ToNode);
				transportRouteElement.Path = path;
			}
			OnPathFound(transportRoute);
			yield return null;
		}
		#endregion
	}

	public class Path
	{
		#region Attributes
		private List<WayPoint> _wayPoints; // Nodes that need to be visited one after the other
		#endregion

		#region Getter & Setter
		public List<WayPoint> WayPoints {
			get {
				return _wayPoints;
			}

			set {
				_wayPoints = value;
			}
		}
		#endregion

		#region Constructor
		public Path()
		{
			WayPoints = new List<WayPoint>();
		}
		#endregion
	}

	public struct WayPoint
	{
		private Vector3[] _traversalVectors;
		private float _radius;

		public WayPoint(Vector3 fromVector3, Vector3 toVector3)
		{
			_traversalVectors = new Vector3[2];
			_traversalVectors[0] = fromVector3;
			_traversalVectors[1] = toVector3;
			_radius = 0f;
		}

		public WayPoint(Vector3 fromVector3, Vector3 offsetVector3, Vector3 toVector3, float radius)
		{
			_traversalVectors = new Vector3[3];
			_traversalVectors[0] = fromVector3;
			_traversalVectors[1] = offsetVector3;
			_traversalVectors[2] = toVector3;
			_radius = radius;
		}

		public Vector3[] TraversalVectors {
			get { return _traversalVectors; }
			set { _traversalVectors = value; }
		}

		public float Radius {
			get { return _radius; }
			set { _radius = value; }
		}

		public override string ToString()
		{
			String output = "TraversalVectors: ";
			foreach (Vector3 traversalVector in _traversalVectors)
			{
				output += traversalVector.ToString() + ", ";
			}
			return output;
		}
	}

	class Node : IHeapItem<Node>
	{
		#region Attributes
		private Node _parent;
		private PathFindingNode _pathFindingNode;
		private int _hCost;
		private int _gCost;
		#endregion

		#region Constructors
		public Node(PathFindingNode pathFindingNode)
		{
			_pathFindingNode = pathFindingNode;
		}

		public Node(PathFindingNode pathFindingNode, int hCost, int gCost)
		{
			_pathFindingNode = pathFindingNode;
			_hCost = hCost;
			_gCost = gCost;
		}
		#endregion

		#region Getter & Setter
		public int FCost {
			get { return HCost + GCost; }
		}

		public int HCost {
			get {
				return _hCost;
			}

			set {
				_hCost = value;
			}
		}

		public int GCost {
			get {
				return _gCost;
			}

			set {
				_gCost = value;
			}
		}

		public PathFindingNode PathFindingNode {
			get {
				return _pathFindingNode;
			}

			set {
				_pathFindingNode = value;
			}
		}

		internal Node Parent {
			get {
				return _parent;
			}

			set {
				_parent = value;
			}
		}

		public int HeapIndex { get; set; }
		#endregion

		#region Methods
		public int CompareTo(Node other)
		{
			int compare = FCost.CompareTo(other.FCost);
			if (compare == 0)
			{
				compare = HCost.CompareTo(other.HCost);
			}

			return -compare;
		}
		#endregion
	}

	public interface IPathFindingAlgorithm
	{
		#region Methods
		Path FindPath(PathFindingNode startNode, PathFindingNode endNode);
		#endregion
	}

	public class AStarPathFinding : IPathFindingAlgorithm
	{
		#region Getter & Setter
		int GetDistance(PathFindingNode nodeA, PathFindingNode nodeB)
		{
			return (int)Math.Abs((nodeA.transform.position - nodeB.transform.position).magnitude);
		}
		#endregion

		#region Methods
		public Path FindPath(PathFindingNode startNode, PathFindingNode endNode)
		{
			Heap<Node> openSet = new Heap<Node>(PathFindingNode.TotalNodeCount);
			HashSet<PathFindingNode> closedSet = new HashSet<PathFindingNode>();
			openSet.Add(new Node(startNode));

			while (openSet.Count > 0)
			{
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode.PathFindingNode);

				if (currentNode.PathFindingNode == endNode)
				{
					return RetracePath(startNode, currentNode);
				}

				foreach (PathFindingNode neighbor in currentNode.PathFindingNode.NeighborNodes)
				{
					if (!neighbor || (!neighbor.IsTraversable() && neighbor != endNode) || closedSet.Contains(neighbor)) continue;

					int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode.PathFindingNode, neighbor);
					Node neighborNode = new Node(neighbor, GetDistance(neighbor, endNode), newMovementCostToNeighbor) { Parent = currentNode };
					if (!openSet.Contains(neighborNode))
					{
						openSet.Add(neighborNode);
					}
					else
					{
						openSet.UpdateItem(neighborNode);
					}
				}
			}
			return null;
		}

		Path RetracePath(PathFindingNode fromNode, Node toNode)
		{
			Path path = new Path();

			Node lastNode = null;
			Node currentNode = toNode;


			while (currentNode.PathFindingNode != fromNode)
			{
				path.WayPoints.Add(CalculateTraversalVectors(lastNode, currentNode));
				lastNode = currentNode;
				currentNode = currentNode.Parent;
			}
			path.WayPoints.Add(CalculateTraversalVectors(lastNode, currentNode));
			path.WayPoints.Reverse();
			return path;
		}

		/// <summary>
		/// Returns the Traversal Vectors as a WayPoint Object from a given set of PathfindingNodes
		/// </summary>
		/// <param name="lastNode"></param>
		/// <param name="currentNode"></param>
		/// <returns></returns>
		WayPoint CalculateTraversalVectors(Node lastNode, Node currentNode)
		{
			Node nextNode = currentNode.Parent;

			Vector3 fromVector3 = new Vector3();
			if (lastNode != null)
			{
				fromVector3 = (lastNode.PathFindingNode.transform.position - currentNode.PathFindingNode.transform.position).normalized;
			}

			Vector3 toVector3 = new Vector3();
			if (nextNode != null)
			{
				toVector3 = (nextNode.PathFindingNode.transform.position - currentNode.PathFindingNode.transform.position).normalized;
			}

			int fromDirection = DirectionVectorToInt(fromVector3);
			int toDirection = DirectionVectorToInt(toVector3);

			//Debug.Log(fromVector3.ToString() + ": " + fromDirection + ", " + toVector3.ToString() + ": " + toDirection);

			return currentNode.PathFindingNode.GetTraversalVectors(toDirection, fromDirection);
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
}