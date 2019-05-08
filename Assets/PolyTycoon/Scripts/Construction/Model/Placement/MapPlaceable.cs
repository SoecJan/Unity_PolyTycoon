using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlaceable : MonoBehaviour
{
    [SerializeField] private Sprite _constructionUiSprite;
    [SerializeField] private string _buildingName; // Name of this building
    
    public Sprite ConstructionUiSprite => _constructionUiSprite;
    public string BuildingName => _buildingName;
    public bool IsDraggable { get; protected set; }
    
    public virtual void Rotate(Vector3 axis, float rotationAmount)
    {
        transform.Rotate(axis, rotationAmount);
    }
}
