using UnityEngine;

/// <summary>
/// MapPlaceables are objects that can be placed in the game by the user.
/// </summary>
public abstract class MapPlaceable : MonoBehaviour
{
    [SerializeField] protected bool _isHighlightable = true; // Used when object is selected
    [SerializeField] private bool _isDraggable;

    public bool IsDraggable
    {
        get => _isDraggable;
        set => _isDraggable = value;
    }

    /// <summary>
    /// Rotates the MapPlaceable by rotationAmount degrees.
    /// </summary>
    /// <param name="axis">The axis of rotation</param>
    /// <param name="rotationAmount">The amount of rotation in degrees. 90f = 90° of rotation.</param>
    public virtual void Rotate(Vector3 axis, float rotationAmount)
    {
        transform.Rotate(axis, rotationAmount);
    }
}
