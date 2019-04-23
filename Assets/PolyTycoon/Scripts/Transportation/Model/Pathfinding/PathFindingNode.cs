﻿using System;
using UnityEngine;

/// <summary>
/// All objects that are supposed to be reached by a vehicle need to have this component.
/// After successful registration at BuildingManager <see cref="BuildingManager"/> this component searches for connected Node, using <see cref="PathFindingNode.OnPlacement"/> in adjacent tiles.
/// <see cref="PathFindingNode.OnDestroy"/> handles the cleanup after a street is destroyed.
/// </summary>
public abstract class PathFindingNode : SimpleMapPlaceable
{

	#region Attributes
	private const int NEIGHBOR_COUNT = 4; // The amount of streets that can be connected to this one. 4 = Grid, 8 = Diagonal

	// Indices in the neighborStreets Array
	public const int Up = 0;
	public const int Right = 1;
	public const int Down = 2;
	public const int Left = 3;

	[SerializeField] private PathFindingNode[] neighborNodes; // Array that holds the reference to the next reachable Node.
	#endregion

	#region Getter & Setter

	protected static BuildingManager BuildingManager { get; set; }

	public PathFindingNode[] NeighborNodes {
		get {
			return neighborNodes;
		}

		set {
			neighborNodes = value;
		}
	}

	public static int TotalNodeCount { get; set; }

	protected virtual Vector3 TraversalOffset
	{
		get;
		set;
	}

	public abstract bool IsTraversable();

	public abstract bool IsNode(); // Returns true if this object is supposed to be a node for path finding

	protected virtual SimpleMapPlaceable AdjacentNodes(int i)
	{
		//	SimpleMapPlaceable mapPlaceableTop = BuildingManager.GetMapPlaceable(gameObject.transform.position + Vector3.forward);
		//	SimpleMapPlaceable mapPlaceableRight = BuildingManager.GetMapPlaceable(gameObject.transform.position + Vector3.right);
		//	SimpleMapPlaceable mapPlaceableBottom = BuildingManager.GetMapPlaceable(gameObject.transform.position + Vector3.back);
		//	SimpleMapPlaceable mapPlaceableLeft = BuildingManager.GetMapPlaceable(gameObject.transform.position + Vector3.left);
		SimpleMapPlaceable neighborPlaceable = null;
		switch (i)
		{
			case 0:
				neighborPlaceable = BuildingManager.GetNode(gameObject.transform.position + Vector3.forward);
				break;
			case 1:
				neighborPlaceable = BuildingManager.GetNode(gameObject.transform.position + Vector3.right);
				break;
			case 2:
				neighborPlaceable = BuildingManager.GetNode(gameObject.transform.position + Vector3.back);
				break;
			case 3:
				neighborPlaceable = BuildingManager.GetNode(gameObject.transform.position + Vector3.left);
				break;
		}

		if (neighborPlaceable && neighborPlaceable is PathFindingNode) return (PathFindingNode)neighborPlaceable;
		return null;
	}

	public float DistanceTo(Transform targetTransform)
	{
		return (transform.position - targetTransform.position).sqrMagnitude;
	}
	#endregion

	#region Default Methods
	protected override void Initialize()
	{
		if (BuildingManager == null) BuildingManager = FindObjectOfType<GroundPlacementController>().BuildingManager;
		
	}

	void OnDrawGizmos()
	{
		
		for (int i = 0; i < NeighborNodes.Length; i++)
		{
			if (NeighborNodes[i])
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(transform.position + (Vector3.up*2), 0.3f);
			}
			foreach (NeededSpace coordinate in UsedCoordinates)
			{
				Gizmos.color = coordinate.TerrainType == TerrainGenerator.TerrainType.Coast ? Color.blue : Color.yellow;
				Gizmos.DrawSphere(gameObject.transform.position + coordinate.UsedCoordinate, 0.5f);
			}	
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.position + UsedCoordinates[0].UsedCoordinate + Vector3.up, 0.3f);
		}
	}

	/// <summary>
	/// Cleans up on this street object after it has been destroyed
	/// </summary>
	void OnDestroy()
	{
		// Remove this street instance from the neighbors
		if (!IsPlaced) return;

		TotalNodeCount -= 1;
	}

	/// <summary>
	/// Searches for neighbor streets after being placed on the map
	/// </summary>
	public override void OnPlacement()
	{
		if (BuildingManager == null) BuildingManager = FindObjectOfType<GroundPlacementController>().BuildingManager;
		TraversalOffset = transform.position;
		NeighborNodes = new PathFindingNode[NEIGHBOR_COUNT];
		TotalNodeCount += 1;
		IsPlaced = true;
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
	}

	/// <summary>
	/// Checks all adjacent Nodes in one direction until a Node is found.
	/// </summary>
	/// <param name="direction"></param>
	/// <returns>The Node that was found in the specified direction</returns>
	protected PathFindingNode[] FindNextNodes()
	{
		PathFindingNode[] pathFindingNodes = new PathFindingNode[NEIGHBOR_COUNT];
		for (int i = 0; i < NEIGHBOR_COUNT; i++)
		{
			PathFindingNode nextNode = AdjacentNodes(i) is PathFindingNode ? (PathFindingNode)AdjacentNodes(i) : null;
			while (nextNode && !nextNode.IsNode())
			{
				Array.Clear(nextNode.NeighborNodes, 0, NEIGHBOR_COUNT);  // Clear Neighbors of non nodes
				nextNode = nextNode.AdjacentNodes(i) is PathFindingNode ? (PathFindingNode)nextNode.AdjacentNodes(i) : null;
			}

			if (nextNode != this) // Bigger Nodes would otherwise add themselves
			{
				pathFindingNodes[i] = nextNode;
			}

			if (pathFindingNodes[i] && IsNode()) // Check if the Node exists and i am a node
			{
				pathFindingNodes[i].NeighborNodes[(i + 2) % NEIGHBOR_COUNT] = this;
			}
		}
		if (IsNode()) // If i am a node > Safe
		{
			NeighborNodes = pathFindingNodes;
		}
		return pathFindingNodes;
	}
	#endregion

	#region Traversal
	///<summary>Returns the TraversalVectors as a WayPoint Object.</summary>
	public virtual WayPoint GetTraversalVectors(int fromDirection, int toDirection)
	{
		
		// Start Points
		if (fromDirection == -1)
		{
			switch (toDirection)
			{
				case Up:
					return new WayPoint(TraversalPoint.CenterBottomRight + TraversalOffset, TraversalPoint.TopRight + TraversalOffset);
				case Right:
					return new WayPoint(TraversalPoint.CenterBottomLeft + TraversalOffset, TraversalPoint.RightBottom + TraversalOffset);
				case Down:
					return new WayPoint(TraversalPoint.CenterTopLeft + TraversalOffset, TraversalPoint.BottomLeft + TraversalOffset);
				case Left:
					return new WayPoint(TraversalPoint.CenterTopRight + TraversalOffset, TraversalPoint.LeftTop + TraversalOffset);
			}
		}

		// End Points

		if (toDirection == -1)
		{
			switch (fromDirection)
			{
				case Up:
					return new WayPoint(TraversalPoint.TopLeft + TraversalOffset, TraversalPoint.CenterBottomLeft + TraversalOffset);
				case Right:
					return new WayPoint(TraversalPoint.RightTop + TraversalOffset, TraversalPoint.CenterTopLeft + TraversalOffset);
				case Down:
					return new WayPoint(TraversalPoint.BottomRight + TraversalOffset, TraversalPoint.CenterTopRight + TraversalOffset);
				case Left:
					return new WayPoint(TraversalPoint.LeftBottom + TraversalOffset, TraversalPoint.CenterBottomRight + TraversalOffset);
			}
		}

		// Straights

		if (fromDirection == Up && toDirection == Down)
		{
			return new WayPoint(TraversalPoint.TopLeft + TraversalOffset, TraversalPoint.BottomLeft + TraversalOffset);
		}
		
		if (fromDirection == Down && toDirection == Up)
		{
			return new WayPoint(TraversalPoint.BottomRight + TraversalOffset, TraversalPoint.TopRight + TraversalOffset);
		}
		
		if (fromDirection == Left && toDirection == Right)
		{
			return new WayPoint(TraversalPoint.LeftBottom + TraversalOffset, TraversalPoint.RightBottom + TraversalOffset);
		}
		
		if (fromDirection == Right && toDirection == Left)
		{
			return new WayPoint(TraversalPoint.RightTop + TraversalOffset, TraversalPoint.LeftTop + TraversalOffset);
		}

		// Inner Corners

		float innerCornerRadius = 0.5f;

		if (fromDirection == Up && toDirection == Left)
		{
			return new WayPoint(TraversalPoint.TopLeft + TraversalOffset, TraversalPoint.CenterTopLeft + TraversalOffset, TraversalPoint.LeftTop + TraversalOffset, innerCornerRadius);
		}

		if (fromDirection == Down && toDirection == Right)
		{
			return new WayPoint(TraversalPoint.BottomRight + TraversalOffset, TraversalPoint.CenterBottomRight + TraversalOffset, TraversalPoint.RightBottom + TraversalOffset, innerCornerRadius);
		}

		if (fromDirection == Left && toDirection == Down)
		{
			return new WayPoint(TraversalPoint.LeftBottom + TraversalOffset, TraversalPoint.CenterBottomLeft + TraversalOffset, TraversalPoint.BottomLeft + TraversalOffset, innerCornerRadius);
		}

		if (fromDirection == Right && toDirection == Up)
		{
			return new WayPoint(TraversalPoint.RightTop + TraversalOffset, TraversalPoint.CenterTopRight + TraversalOffset, TraversalPoint.TopRight + TraversalOffset, innerCornerRadius);
		}

		// Outer Corners

		float outerCornerRadius = 0.75f;

		if (fromDirection == Up && toDirection == Right)
		{
			return new WayPoint(TraversalPoint.TopLeft + TraversalOffset, TraversalPoint.CenterBottomLeft + TraversalOffset, TraversalPoint.RightBottom + TraversalOffset, outerCornerRadius);
		}

		if (fromDirection == Down && toDirection == Left)
		{
			return new WayPoint(TraversalPoint.BottomRight + TraversalOffset, TraversalPoint.CenterTopRight + TraversalOffset, TraversalPoint.LeftTop + TraversalOffset, outerCornerRadius);
		}

		if (fromDirection == Left && toDirection == Up)
		{
			return new WayPoint(TraversalPoint.LeftBottom + TraversalOffset, TraversalPoint.CenterBottomRight + TraversalOffset, TraversalPoint.TopRight + TraversalOffset, outerCornerRadius);
		}

		if (fromDirection == Right && toDirection == Down)
		{
			return new WayPoint(TraversalPoint.RightTop + TraversalOffset, TraversalPoint.CenterTopLeft + TraversalOffset, TraversalPoint.BottomLeft + TraversalOffset, outerCornerRadius);
		}
		Debug.LogError("Should not reach here! Input: " + fromDirection + "; " + toDirection);
		return new WayPoint(Vector3.zero, Vector3.zero);
	}
	#endregion
}

public struct TraversalPoint
{
	// Center points
	private static Vector3 middleLeft = new Vector3(-0.5f, 0f, 0f);
	private static Vector3 middleRight = new Vector3(0.5f, 0f, 0f);
	private static Vector3 middleTop = new Vector3(0f, 0f, 0.5f);
	private static Vector3 middleBottom = new Vector3(0f, 0f, -0.5f);
	
	// Lane points
	private static Vector3 topLeft = new Vector3(-0.25f, 0f, 0.5f);
	private static Vector3 topRight = new Vector3(0.25f, 0f, 0.5f);
	private static Vector3 rightTop = new Vector3(0.5f, 0f, 0.25f);
	private static Vector3 rightBottom = new Vector3(0.5f, 0f, -0.25f);
	private static Vector3 bottomRight = new Vector3(0.25f, 0f, -0.5f);
	private static Vector3 bottomLeft = new Vector3(-0.25f, 0f, -0.5f);
	private static Vector3 leftBottom = new Vector3(-0.5f, 0f, -0.25f);
	private static Vector3 leftTop = new Vector3(-0.5f, 0f, 0.25f);

	// Turn helper
	private static Vector3 centerTopRight = new Vector3(0.25f, 0f, 0.25f);
	private static Vector3 centerTopLeft = new Vector3(-0.25f, 0f, 0.25f);
	private static Vector3 centerBottomRight = new Vector3(0.25f, 0f, -0.25f);
	private static Vector3 centerBottomLeft = new Vector3(-0.25f, 0f, -0.25f);

	public static Vector3 TopLeft {
		get { return topLeft; }
		set { topLeft = value; }
	}

	public static Vector3 TopRight {
		get { return topRight; }
		set { topRight = value; }
	}

	public static Vector3 RightTop {
		get { return rightTop; }
		set { rightTop = value; }
	}

	public static Vector3 RightBottom {
		get { return rightBottom; }
		set { rightBottom = value; }
	}

	public static Vector3 BottomRight {
		get { return bottomRight; }
		set { bottomRight = value; }
	}

	public static Vector3 BottomLeft {
		get { return bottomLeft; }
		set { bottomLeft = value; }
	}

	public static Vector3 LeftBottom {
		get { return leftBottom; }
		set { leftBottom = value; }
	}

	public static Vector3 LeftTop {
		get { return leftTop; }
		set { leftTop = value; }
	}

	public static Vector3 CenterTopRight {
		get { return centerTopRight; }
		set { centerTopRight = value; }
	}

	public static Vector3 CenterTopLeft {
		get { return centerTopLeft; }
		set { centerTopLeft = value; }
	}

	public static Vector3 CenterBottomRight {
		get { return centerBottomRight; }
		set { centerBottomRight = value; }
	}

	public static Vector3 CenterBottomLeft {
		get { return centerBottomLeft; }
		set { centerBottomLeft = value; }
	}

	public static Vector3 MiddleLeft
	{
		get { return middleLeft; }
	}

	public static Vector3 MiddleRight
	{
		get { return middleRight; }
	}

	public static Vector3 MiddleTop
	{
		get { return middleTop; }
	}

	public static Vector3 MiddleBottom
	{
		get { return middleBottom; }
	}
}