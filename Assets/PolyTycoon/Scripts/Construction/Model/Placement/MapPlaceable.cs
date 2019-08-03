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

    public Sprite ConstructionUiSprite => _constructionUiSprite;
    public string BuildingName => _buildingName;
    public bool IsDraggable { get; set; }

    public Outline Outline { get; private set; }

    void Start()
    {
        Outline = GetComponent<Outline>();
        if (Outline) return;
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
