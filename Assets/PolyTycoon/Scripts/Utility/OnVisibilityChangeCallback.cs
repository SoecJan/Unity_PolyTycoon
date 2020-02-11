using UnityEngine;

/// <summary>
/// Component that can be added to a GameObject. The associated Object needs to have a renderer component. 
/// </summary>
public class OnVisibilityChangeCallback : MonoBehaviour
{
    public System.Action<bool> OnVisibilityChange;
    
    void OnBecameInvisible()
    {
        OnVisibilityChange?.Invoke(false);
    }
    
    void OnBecameVisible()
    {
        OnVisibilityChange?.Invoke(true);
    }
}
