using UnityEngine;

/// <summary>
/// MapPlaceables are objects that can be placed in the game by the user.
/// </summary>
public abstract class MapPlaceable : MonoBehaviour, IMapPlaceable
{
    [SerializeField] protected bool _isHighlightable = true; // Used when object is selected

    protected Renderer childRenderer; // Create a new instance of the material
    protected static readonly int IsPlacedProperty = Shader.PropertyToID("_IsPlaced");
    protected static readonly int IsPlaceableProperty = Shader.PropertyToID("_IsPlaceable");
    
    public bool IsDraggable { get; set; }
    
    protected Material RendererMaterial { get; set; }

    public bool IsPlaceable
    {
        set => RendererMaterial.SetFloat(IsPlaceableProperty, value ? 1f : 0f);
    }

    public Outline Outline { get; private set; }

    public virtual void Awake()
    {
        childRenderer = GetComponentInChildren<Renderer>();
        if (!childRenderer) return;
        RendererMaterial = new Material(childRenderer.material);
        childRenderer.material = RendererMaterial;
    }

    public virtual void Start()
    {
        Outline = GetComponent<Outline>();
        if (Outline || !_isHighlightable) return;
        Outline = gameObject.AddComponent<Outline>();
        Outline.OutlineMode = Outline.Mode.OutlineVisible;
        Outline.OutlineColor = Color.yellow;
        Outline.OutlineWidth = 5f;
        Outline.enabled = false;
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
