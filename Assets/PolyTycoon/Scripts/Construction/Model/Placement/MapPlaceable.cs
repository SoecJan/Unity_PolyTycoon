using UnityEngine;


public interface IMapPlaceable
{
    Sprite ConstructionUiSprite { get; }
    string BuildingName { get; }
    bool IsDraggable { get; set; }
}

public abstract class MapPlaceable : MonoBehaviour, IMapPlaceable
{
    [SerializeField] private Sprite _constructionUiSprite;
    [SerializeField] private string _buildingName; // Name of this building
    [SerializeField] protected bool _isHighlightable = true;
    
    public Sprite ConstructionUiSprite => _constructionUiSprite;
    public string BuildingName { get => _buildingName; set => _buildingName = value; }
    public bool IsDraggable { get; set; }

    public Outline Outline { get; private set; }

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

    public virtual void Rotate(Vector3 axis, float rotationAmount)
    {
        transform.Rotate(axis, rotationAmount);
    }
}
