using UnityEngine;

public interface IWorldToScreenManager
{
    WorldToScreenElement Add(GameObject uiPrefab, Transform anchorTransform, Vector3 offset);
}