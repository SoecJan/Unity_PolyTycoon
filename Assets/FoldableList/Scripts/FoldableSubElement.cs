using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoldableSubElement : MonoBehaviour
{
    [SerializeField] private GameObject _childPrefab;
    [SerializeField] private RectTransform _subItemTransform;
    private LayoutElement _layoutElement;
    private List<GameObject> _instantiatedChildPrefabs;

    public GameObject ChildPrefab
    {
        get => _childPrefab;
        set => _childPrefab = value;
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (_subItemTransform == null) _subItemTransform = (RectTransform) transform.GetChild(0);
        _layoutElement = GetComponent<LayoutElement>();
        _instantiatedChildPrefabs = new List<GameObject>();
    }

    private float GetHeight()
    {
        float height = 0;
        foreach (GameObject child in _instantiatedChildPrefabs)
        {
            height += child.GetComponent<LayoutElement>().preferredHeight;
        }
        return height;
    }

    public void Add()
    {
        GameObject addedObj = Instantiate(_childPrefab, _subItemTransform);
        _instantiatedChildPrefabs.Add(addedObj);
        _layoutElement.preferredHeight = GetHeight();
    }

    public void Remove(int i)
    {
        Destroy(_instantiatedChildPrefabs[i]);
        _instantiatedChildPrefabs.RemoveAt(i);
    }
}
