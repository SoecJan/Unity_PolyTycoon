using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// All objects that can be placed on the map need to have this or a derivative of this component. 
/// Placement on the grid is then processed by <see cref="GroundPlacementController"/> and registered by <see cref="BuildingManager"/>.
/// </summary>
public abstract class SimpleMapPlaceable : MonoBehaviour
{

	#region Attributes

	private bool _isDraggable;
	protected bool IsClickable;
	private static System.Action<SimpleMapPlaceable> _onClickAction;
	[SerializeField] private Sprite _constructionUiSprite;
	[SerializeField] private List<NeededSpace> _usedCoordinates; // All coordinates that are blocked relative to this transform
	[SerializeField] private string _buildingName; // Name of this building

	private bool _isPlaced = false; // Whether it is placed on the map or not
	[SerializeField] private Vector3 _threadsafePosition;

	#endregion

	#region Default Methods
	/// <summary>
	/// Gets the static reference to BuildingManager instance
	/// </summary>
	void Awake()
	{
		Initialize();
	}

	protected abstract void Initialize();

	/// <summary>
	/// Draws the UsedCoordinates for debugging
	/// </summary>
	void OnDrawGizmos()
	{
		foreach (NeededSpace coordinate in UsedCoordinates)
		{
			Gizmos.color = coordinate.TerrainType == TerrainGenerator.TerrainType.Coast ? Color.blue : Color.yellow;
			Gizmos.DrawSphere(gameObject.transform.position + coordinate.UsedCoordinate, 0.5f);
		}	
	}

	/// <summary>
	/// Detects if a placeable was clicked on by the player.
	/// Prevents any detection if the palceable was just placed by <see cref="GroundPlacementController"/>.
	/// </summary>
	void OnMouseOver()
	{
		if (IsClickable && Input.GetMouseButtonDown(0) && IsPlaced && !DestructionController.DestructionActive && !EventSystem.current.IsPointerOverGameObject())
		{
			_onClickAction(this);
		}
	}
	#endregion

	#region Getter & Setter

	public Vector3 ThreadsafePosition
	{
		get => _threadsafePosition.Equals(default(Vector3)) ? throw new NotImplementedException() : _threadsafePosition;

		set => _threadsafePosition = value;
	}

	public List<NeededSpace> UsedCoordinates {
		get {
			return _usedCoordinates;
		}

		set {
			_usedCoordinates = value;
		}
	}

	public string BuildingName {
		get {
			return _buildingName;
		}

		set {
			_buildingName = value;
		}
	}

	public bool IsPlaced {
		get {
			return _isPlaced;
		}

		set {
			_isPlaced = value;
		}
	}

	public Sprite ConstructionUiSprite {
		get {
			return _constructionUiSprite;
		}

		set {
			_constructionUiSprite = value;
		}
	}

	public bool IsDraggable {
		get {
			return _isDraggable;
		}

		set {
			_isDraggable = value;
		}
	}

	public Action<SimpleMapPlaceable> OnClickAction {
		get {
			return _onClickAction;
		}

		set {
			_onClickAction = value;
		}
	}

	public virtual float GetHeight()
	{
		return transform.lossyScale.y;
	}
	#endregion

	#region MapPlaceable common
	/// <summary>
	/// Is called by <see cref="BuildingManager"/> after successful placement of this MapPlaceable.
	/// </summary>
	public virtual void OnPlacement()
	{
		IsPlaced = true;
		ThreadsafePosition = transform.position;
	}

	/// <summary>
	/// Rotates the UsedCoordinates to align to the current Transform rotation. 
	/// Called before Placement by <see cref="GroundPlacementController"/>.
	/// </summary>
	protected void RotateUsedCoords(float rotationAmount)
	{
		for (int i = 0; i < _usedCoordinates.Count; i++)
		{
			Vector3 rotatedOffset = Quaternion.Euler(0, rotationAmount, 0) * _usedCoordinates[i].UsedCoordinate;
			_usedCoordinates[i].UsedCoordinate = Vector3Int.RoundToInt(rotatedOffset);
		}
	}

	public virtual void Rotate(Vector3 axis, float rotationAmount)
	{
		transform.Rotate(axis, rotationAmount);
		RotateUsedCoords(rotationAmount);
	}
	#endregion
}

[Serializable]
public class NeededSpace
{
	[SerializeField] private Vector3Int _usedCoordinate;
	[SerializeField] private TerrainGenerator.TerrainType _terrainType = TerrainGenerator.TerrainType.Flatland;

	public NeededSpace(NeededSpace neededSpace, Vector3Int offset)
	{
		_usedCoordinate = new Vector3Int(neededSpace.UsedCoordinate.x + offset.x, neededSpace.UsedCoordinate.y + offset.y, neededSpace.UsedCoordinate.z + offset.z);
		_terrainType = neededSpace.TerrainType;
	}
	
	public Vector3Int UsedCoordinate
	{
		get { return _usedCoordinate; }
		set { _usedCoordinate = value; }
	}

	public TerrainGenerator.TerrainType TerrainType
	{
		get { return _terrainType; }
		set { _terrainType = value; }
	}
}