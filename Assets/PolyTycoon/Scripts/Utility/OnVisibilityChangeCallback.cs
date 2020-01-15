using UnityEngine;

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
