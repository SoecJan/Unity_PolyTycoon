using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlaceable : MonoBehaviour
{
    [SerializeField] private Sprite _constructionUiSprite;
    [SerializeField] private string _buildingName; // Name of this building
    private Outline _outline;
    
    public Sprite ConstructionUiSprite => _constructionUiSprite;
    public string BuildingName => _buildingName;
    public bool IsDraggable { get; protected set; }

    public Outline Outline
    {
        get { return _outline; }
        set { _outline = value; }
    }

    void Start()
    {
        Outline = GetComponent<Outline>();
        if (!Outline)
        {
            Outline = gameObject.AddComponent<Outline>();
            Outline.OutlineMode = Outline.Mode.OutlineVisible;
            Outline.OutlineColor = Color.yellow;
            Outline.OutlineWidth = 5f;
            Outline.enabled = false;
        }
    }

    public virtual void Rotate(Vector3 axis, float rotationAmount)
    {
        transform.Rotate(axis, rotationAmount);
    }
}
