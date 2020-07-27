using UnityEngine;

/// <summary>
/// MapPlaceables are objects that can be placed in the game by the user.
/// </summary>
public abstract class MapPlaceable : MonoBehaviour, IMapPlaceable
{
    [SerializeField] protected bool _isHighlightable = true; // Used when object is selected

    protected Renderer[] childRenderers; // Create a new instance of the material
    protected static readonly int IsPlacedProperty = Shader.PropertyToID("_IsPlaced");
    protected static readonly int IsPlaceableProperty = Shader.PropertyToID("_IsPlaceable");
    protected MaterialPropertyBlock materialPropertyBlock;
    
    public bool IsDraggable { get; set; }

    public bool IsPlaceable
    {
        set
        {
            materialPropertyBlock.SetFloat(IsPlacedProperty, 0f);
            materialPropertyBlock.SetFloat(IsPlaceableProperty, value ? 1f : 0f);
            foreach (Renderer childRenderer in childRenderers)
            {
                // childRenderer.SetPropertyBlock(null);
                childRenderer.SetPropertyBlock(materialPropertyBlock);
            }
        }
    }

    public virtual void Awake()
    {
        childRenderers = GetComponentsInChildren<Renderer>();
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    public virtual void Start()
    {
        
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
