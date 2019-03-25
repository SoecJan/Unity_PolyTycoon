using System;
using Assets.PolyTycoon.Scripts.Construction.Model.Factory;
using Assets.PolyTycoon.Scripts.Construction.Model.Placement;
using UnityEngine;

/// <summary>
/// All objects that are supposed to be reached by a vehicle need to have this component.
/// After successful registration at BuildingManager <see cref="BuildingManager"/> this component searches for connected Node, using <see cref="PathFindingNode.OnPlacement"/> in adjacent tiles.
/// <see cref="PathFindingNode.OnDestroy"/> handles the cleanup after a street is destroyed.
/// </summary>
public abstract class PathFindingNode : SimpleMapPlaceable
{
	#region Attributes
	private static int _totalNodeCount;
	private const int NEIGHBOR_COUNT = 4; // The amount of streets that can be connected to this one. 4 = Grid, 8 = Diagonal

	// Indices in the neighborStreets Array
	public const int TOP_NODE = 0;
	public const int RIGHT_NODE = 1;
	public const int BOTTOM_NODE = 2;
	public const int LEFT_NODE = 3;

	private static BuildingManager buildingManager; // Used to search for connected streets to this object
	[SerializeField] private PathFindingNode[] neighborNodes; // Array that holds the reference to the next reachable Node.
	#endregion

	#region Getter & Setter

	public static BuildingManager BuildingManager {
		get {
			return buildingManager;
		}

		set {
			buildingManager = value;
		}
	}

	public PathFindingNode[] NeighborNodes {
		get {
			return neighborNodes;
		}

		set {
			neighborNodes = value;
		}
	}

	public static int TotalNodeCount {
		get {
			return _totalNodeCount;
		}

		set {
			_totalNodeCount = value;
		}
	}

	public abstract bool IsTraversable();

	public abstract bool IsNode(); // Returns true if this object is supposed to be a node for path finding

	public SimpleMapPlaceable AdjacentNodes(int i)
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
			Gizmos.color = Color.blue;
			foreach (Vector3 usedCoordinate in UsedCoordinates)
			{
				Gizmos.DrawSphere(transform.position + usedCoordinate, 0.3f);
			}
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.position + UsedCoordinates[0] + Vector3.up, 0.3f);
		}

		//Gizmos.color = Color.yellow;
		//foreach (Vector3 coordinate in UsedCoordinates)
		//	Gizmos.DrawSphere(gameObject.transform.position + coordinate, 0.5f);

		//Gizmos.color = Color.green;
		//if (IsTraversable())
		//	Gizmos.DrawCube(transform.position + new Vector3(0, -(transform.lossyScale.y / 2f) + 0.2f, 0), new Vector3(1f, 0.4f, 1f));

		//Gizmos.color = Color.blue;
		//if (IsNode())
		//	Gizmos.DrawCube(transform.position, new Vector3(1f, 0.4f, 1f));
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
	public PathFindingNode[] FindNextNodes()
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

	/// <summary>
	/// Checks the BuildingManager for surrounding tiles that are a street object and registers those as neighbors
	/// </summary>
	//void FindAdjacentNodes()
	//{
	//	if (BuildingManager == null) BuildingManager = FindObjectOfType<GroundPlacementController>().BuildingManager;
	//	// Find all neighbor streets
	//	SimpleMapPlaceable mapPlaceableTop = BuildingManager.GetMapPlaceable(gameObject.transform.position + Vector3.forward);
	//	SimpleMapPlaceable mapPlaceableRight = BuildingManager.GetMapPlaceable(gameObject.transform.position + Vector3.right);
	//	SimpleMapPlaceable mapPlaceableBottom = BuildingManager.GetMapPlaceable(gameObject.transform.position + Vector3.back);
	//	SimpleMapPlaceable mapPlaceableLeft = BuildingManager.GetMapPlaceable(gameObject.transform.position + Vector3.left);

	//	// Get a reference to the neighbor object and set this street on the neighbor object
	//	// Top Street
	//	if (mapPlaceableTop && mapPlaceableTop is PathFindingNode)
	//	{
	//		AdjacentNodes[TOP_NODE] = (PathFindingNode)mapPlaceableTop;
	//		AdjacentNodes[TOP_NODE].AdjacentNodes[BOTTOM_NODE] = this;
	//	}
	//	// Right Street
	//	if (mapPlaceableRight && mapPlaceableRight is PathFindingNode)
	//	{
	//		AdjacentNodes[RIGHT_NODE] = (PathFindingNode)mapPlaceableRight;
	//		AdjacentNodes[RIGHT_NODE].AdjacentNodes[LEFT_NODE] = this;
	//	}
	//	// Bottom Street
	//	if (mapPlaceableBottom && mapPlaceableBottom is PathFindingNode)
	//	{
	//		AdjacentNodes[BOTTOM_NODE] = (PathFindingNode)mapPlaceableBottom;
	//		AdjacentNodes[BOTTOM_NODE].AdjacentNodes[TOP_NODE] = this;
	//	}
	//	// Left Street
	//	if (mapPlaceableLeft && mapPlaceableLeft is PathFindingNode)
	//	{
	//		AdjacentNodes[LEFT_NODE] = (PathFindingNode)mapPlaceableLeft;
	//		AdjacentNodes[LEFT_NODE].AdjacentNodes[RIGHT_NODE] = this;
	//	}
	//}
	#endregion
}
