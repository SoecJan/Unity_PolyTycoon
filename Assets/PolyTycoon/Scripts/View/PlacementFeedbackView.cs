using UnityEngine;

public class PlacementFeedbackView : MonoBehaviour
{
    protected Renderer[] childRenderers; // Create a new instance of the material
    protected static readonly int IsPlacedProperty = Shader.PropertyToID("_IsPlaced");
    protected static readonly int IsPlaceableProperty = Shader.PropertyToID("_IsPlaceable");
    protected MaterialPropertyBlock materialPropertyBlock;

    void Awake()
    {
        childRenderers = GetComponentsInChildren<Renderer>();
        materialPropertyBlock = new MaterialPropertyBlock();
    }
    
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

    void OnDestroy()
    {
        materialPropertyBlock.SetFloat(IsPlacedProperty, 1f);
        foreach (Renderer childRenderer in childRenderers)
        {
            // childRenderer.SetPropertyBlock(null);
            childRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}
