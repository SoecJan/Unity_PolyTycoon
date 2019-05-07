using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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
			return simpleMapPlaceable ? (simpleMapPlaceable as PathFindingNode) : null;
		}
		else
		{
			return null;
		}
	}
	#endregion

	#region Default Methods
	protected override void Initialize()
	{
		if (BuildingManager == null) BuildingManager = FindObjectOfType<GroundPlacementController>().BuildingManager;
	}

	void OnDrawGizmos()
	{
		Vector3 position = Vector3.zero;
		foreach (NeededSpace coordinate in UsedCoordinates)
		{
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
	private PathFindingNode[] FindNextNodes()
	{
		PathFindingNode[] pathFindingNodes = new PathFindingNode[NEIGHBOR_COUNT];
		for (int i = 0; i < NEIGHBOR_COUNT; i++)
		{
			PathFindingNode nextNode = AdjacentNodes(i) as PathFindingNode;
			while (nextNode && !nextNode.IsNode())
			{
				Array.Clear(nextNode.NeighborNodes, 0, NEIGHBOR_COUNT);  // Clear Neighbors of non nodes
				nextNode = nextNode.AdjacentNodes(i) as PathFindingNode;
			}

			if (nextNode != this) // Nodes would otherwise add themselves
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

	public abstract WayPoint GetTraversalVectors(int fromDirection, int toDirection);
}