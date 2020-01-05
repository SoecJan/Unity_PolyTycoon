using UnityEngine;

/// <summary>
/// This interface describes the functionality of a MapPlaceable.
/// </summary>
public interface IMapPlaceable
{
    /// <summary>
    /// The sprite that is displayed to the user and associated with this MapPlaceable
    /// </summary>
    Sprite ConstructionUiSprite { get; }
    /// <summary>
    /// The name that is associated with this MapPlaceable
    /// </summary>
    string BuildingName { get; }
    /// <summary>
    /// If the associated MapPlaceable can be dragged to created multiple instances in one line.
    /// </summary>
    bool IsDraggable { get; set; }
}

/// <summary>
/// MapPlaceables are objects that can be placed in the game by the user.
/// </summary>
public abstract class MapPlaceable : MonoBehaviour, IMapPlaceable
{
    [SerializeField] private Sprite _constructionUiSprite; // Sprite used for construction ui
    [SerializeField] private string _buildingName; // Name of this building
    [SerializeField] private int _buildingPrice;
    [SerializeField] protected bool _isHighlightable = true; // Used when object is selected
    protected static MoneyUiController _moneyUiController; // Controller that handles the players money
    
    public Sprite ConstructionUiSprite => _constructionUiSprite;
    public string BuildingName { get => _buildingName; set => _buildingName = value; }
    public bool IsDraggable { get; set; }

    public Outline Outline { get; private set; }

    public int BuildingPrice
    {
        get => _buildingPrice;
        set => _buildingPrice = value;
    }

    public virtual void Start()
    {
        if (!_moneyUiController) _moneyUiController = FindObjectOfType<MoneyUiController>();
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
