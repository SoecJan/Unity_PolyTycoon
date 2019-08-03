using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public interface ISimpleMapPlaceable
{
    Vector3 ThreadsafePosition { get; }
    List<NeededSpace> UsedCoordinates { get; }
    float GetHeight();

    /// <summary>
    /// Is called by <see cref="BuildingManager"/> after successful placement of this MapPlaceable.
    /// </summary>
    void OnPlacement();
}

/// <summary>
/// All objects that can be placed on the map need to have this or a derivative of this component. 
/// Placement on the grid is then processed by <see cref="PlacementManager"/> and registered by <see cref="BuildingManager"/>.
/// </summary>
public abstract class SimpleMapPlaceable : MapPlaceable, ISimpleMapPlaceable
{
    #region Attributes
    protected bool _isClickable;
    [SerializeField]
    private List<NeededSpace> _usedCoordinates; // All coordinates that are blocked relative to this transform
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
    /// Prevents any detection if the placeable was just placed by <see cref="PlacementManager"/>.
    /// </summary>
    void OnMouseOver()
    {
        if (_isClickable && Input.GetMouseButtonDown(0) && IsPlaced && !DestructionController.DestructionActive &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            OnClickAction(this);
        }
    }
    #endregion

    #region Getter & Setter
    public Vector3 ThreadsafePosition
    {
        get => _threadsafePosition.Equals(default(Vector3)) ? throw new NotImplementedException() : _threadsafePosition;

        private set => _threadsafePosition = value;
    }

    public List<NeededSpace> UsedCoordinates => _usedCoordinates;

    protected bool IsPlaced { get; private set; }

    public static Action<SimpleMapPlaceable> OnClickAction { get; set; }

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
    /// Called before Placement by <see cref="PlacementManager"/>.
    /// </summary>
    protected void RotateUsedCoords(float rotationAmount)
    {
        foreach (NeededSpace neededSpace in _usedCoordinates)
        {
            Vector3 rotatedOffset = Quaternion.Euler(0, rotationAmount, 0) * neededSpace.UsedCoordinate;
            neededSpace.UsedCoordinate = Vector3Int.RoundToInt(rotatedOffset);
        }
    }

    public override void Rotate(Vector3 axis, float rotationAmount)
    {
        base.Rotate(axis, rotationAmount);
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
        _usedCoordinate = new Vector3Int(neededSpace.UsedCoordinate.x + offset.x,
            neededSpace.UsedCoordinate.y + offset.y, neededSpace.UsedCoordinate.z + offset.z);
        _terrainType = neededSpace.TerrainType;
    }

    public Vector3Int UsedCoordinate
    {
        get => _usedCoordinate;
        set => _usedCoordinate = value;
    }

    public TerrainGenerator.TerrainType TerrainType => _terrainType;
}