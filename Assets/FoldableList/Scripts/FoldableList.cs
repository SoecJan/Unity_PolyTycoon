using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoldableList : MonoBehaviour
{
    [SerializeField] private FoldableElement _foldableElementPrefab;
    [SerializeField] private List<FoldableElement> _foldableElements;
    private FoldableElement _currentUnfolded;
    
    // Start is called before the first frame update
    void Start()
    {
        _foldableElements = new List<FoldableElement>();
    }

    public FoldableElement Add()
    {
        FoldableElement foldableElement = Instantiate(_foldableElementPrefab, transform);
        foldableElement.OnFoldAction += delegate(FoldableElement element, bool isFolded)
        {
            if (!isFolded)
            {
                _currentUnfolded = null;
                return;
            }

            if (_currentUnfolded == null)
            {
                _currentUnfolded = element;
            }
            else
            {
                _currentUnfolded.Fold(true);
                _currentUnfolded = element;
            }
        };
        _foldableElements.Add(foldableElement);
        return foldableElement;
    }

    public void Clear()
    {
        for (int i = 0; i < _foldableElements.Count; i++)
        {
            Destroy(_foldableElements[i].gameObject);
        }
        _foldableElements.Clear();
    }
}
