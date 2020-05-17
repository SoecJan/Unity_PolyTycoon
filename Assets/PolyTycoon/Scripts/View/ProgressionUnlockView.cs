using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionUnlockView : MonoBehaviour
{
    [SerializeField] private Transform _unlockedParentTransform;
    [SerializeField] private Animator _animator;

    [SerializeField] private ProgressionUnlockViewElement _unlockElementPrefab;

    private List<ProgressionUnlockViewElement> _unlockElements;
    private static readonly int _openAnimation = Animator.StringToHash("Open");
    
    private void Start()
    {
        _unlockElements = new List<ProgressionUnlockViewElement>();
        GameHandler gameHandler = FindObjectOfType<GameHandler>();
        gameHandler.ProgressionManager.onBuildingUnlock += delegate(BuildingData[] buildingDatas)
        {
            StartCoroutine(DisplayUnlocks(buildingDatas));
        };
    }

    private IEnumerator DisplayUnlocks(BuildingData[] unlockedBuildings)
    {
        Debug.Log("Unlock Triggered");
        for (int i = 0; i < _unlockElements.Count; i++)
        {
            Destroy(_unlockElements[i].gameObject);
            _unlockElements.RemoveAt(i);
        }
        _animator.SetBool(_openAnimation, true);
        foreach (BuildingData unlockedBuilding in unlockedBuildings)
        {
            yield return new WaitForSeconds(0.1f);
            ProgressionUnlockViewElement unlockViewElement =
                Instantiate(_unlockElementPrefab, _unlockedParentTransform);
            Debug.Log(unlockedBuilding.BuildingName);
            Debug.Log(unlockViewElement.Image);
            unlockViewElement.Image.sprite = unlockedBuilding.ConstructionSprite;
            _unlockElements.Add(unlockViewElement);
        }
        
        yield return new WaitForSeconds(3f);
        _animator.SetBool(_openAnimation, false);
    }
}