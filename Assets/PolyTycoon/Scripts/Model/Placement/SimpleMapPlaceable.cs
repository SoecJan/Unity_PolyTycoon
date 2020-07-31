using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This interface describes functionality for an object that can be placed inside the game.
/// </summary>
public interface ISimpleMapPlaceable
{
    /// <summary>
    /// A position vector that can be used by other threads.
    /// </summary>
    Vector3 ThreadsafePosition { get; }
    /// <summary>
    /// Coordinates that this objects blocks. Used by the <see cref="BuildingManager"/>.
    /// </summary>
    List<NeededSpace> UsedCoordinates { get; }
    
    /// <returns>The height of the placed object.</returns>
    float GetHeight();

    /// <summary>
    /// Is called by <see cref="BuildingManager"/> after successful placement of this MapPlaceable.
    /// </summary>
    void OnPlacement();

    GameObject gameObject { get; }
}

/// <summary>
/// All objects that can be placed on the map need to have this or a derivative of this component. 
/// Placement on the grid is then processed by <see cref="PlacementController"/> and registered by <see cref="BuildingManager"/>.
/// </summary>
public class SimpleMapPlaceable : MapPlaceable, ISimpleMapPlaceable
{
    #region Attributes
    [SerializeField] protected bool _isClickable;
    [SerializeField] private bool _isRotateable;
    private bool _isPlaced;
    [SerializeField] private List<NeededSpace> _usedCoordinates; // All coordinates that are blocked relative to this transform
    private Vector3 _threadsafePosition;
    public System.Action<SimpleMapPlaceable> _OnPlacementEvent;

    #endregion

    
    #region Default Methods

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
    /// Prevents any detection if the placeable was just placed by <see cref="PlacementController"/>.
    /// </summary>
    void OnMouseOver()
    {
        if (_isClickable && Input.GetMouseButtonDown(0) && IsPlaced && !DestructionController.DestructionActive &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            OnClickAction(this);
        }
    }

    protected virtual void OnMouseEnter()
    {
        if (!_isHighlightable) return;
        Outline outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 5f;
        outline.enabled = true;
    }

    protected virtual void OnMouseExit()
    {
        if (!_isHighlightable) return;
        Destroy(gameObject.GetComponent<Outline>());
    }

    #endregion

    #region Getter & Setter
    public Vector3 ThreadsafePosition
    {
        get => _threadsafePosition.Equals(default(Vector3)) ? throw new NotImplementedException() : _threadsafePosition;

        private set => _threadsafePosition = value;
    }

    public List<NeededSpace> UsedCoordinates
    {
        get => _usedCoordinates;
        set => _usedCoordinates = value;
    }

    public bool IsPlaced
    {
        get => _isPlaced;

        private set
        {
            this._isPlaced = value;
            if (this.IsPlaced)
            {
                // materialPropertyBlock.SetFloat(IsPlacedProperty, 1f);
                // foreach (Renderer childRenderer in childRenderers)
                // {
                //     // childRenderer.SetPropertyBlock(null);
                //     childRenderer.SetPropertyBlock(materialPropertyBlock);
                // }
            }
        }
    }

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
        _OnPlacementEvent?.Invoke(this);
    }

    /// <summary>
    /// Rotates the UsedCoordinates to align to the current Transform rotation. 
    /// Called before Placement by <see cref="PlacementController"/>.
    /// </summary>
    protected void RotateUsedCoords(float rotationAmount)
    {
        if (_usedCoordinates == null) Debug.LogError(gameObject.name);
        foreach (NeededSpace neededSpace in _usedCoordinates)
        {
            Vector3 rotatedOffset = Quaternion.Euler(0, rotationAmount, 0) * neededSpace.UsedCoordinate;
            neededSpace.UsedCoordinate = Vector3Int.RoundToInt(rotatedOffset);
        }
    }

    public override void Rotate(Vector3 axis, float rotationAmount)
    {
        if (!_isRotateable) return;
        base.Rotate(axis, rotationAmount);
        RotateUsedCoords(rotationAmount);
    }

    #endregion
}

/// <summary>
/// Wrapper for used coordinates. This is needed to specify the type of ground that is suitable for a given SimpleMapPlaceable.
/// </summary>
[Serializable]
public class NeededSpace
{
    [SerializeField] private Vector3Int _usedCoordinate; // The relative offset from the origin
    [SerializeField] private TerrainGenerator.TerrainType _terrainType = TerrainGenerator.TerrainType.Flatland; // The suitable ground type

    public NeededSpace(Vector3Int usedCoordinate, TerrainGenerator.TerrainType terrainType)
    {
        this._usedCoordinate = usedCoordinate;
        this._terrainType = terrainType;
    }

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

    public static NeededSpace Zero(TerrainGenerator.TerrainType terrainType)
    {
        return new NeededSpace(Vector3Int.zero, terrainType);
    }
}

[Serializable]
public class ProceduralNeededSpace : NeededSpace
{
    [SerializeField] private float _noiseValue;
    public ProceduralNeededSpace(Vector3Int usedCoordinate, TerrainGenerator.TerrainType terrainType, float noiseValue) : base(usedCoordinate, terrainType)
    {
        this._noiseValue = noiseValue;
    }

    public ProceduralNeededSpace(NeededSpace neededSpace, Vector3Int offset, float noiseValue) : base(neededSpace, offset)
    {
        this._noiseValue = noiseValue;
    }

    public float NoiseValue
    {
        get => _noiseValue;
        set => _noiseValue = value;
    }
}