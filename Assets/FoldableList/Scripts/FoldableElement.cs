using UnityEngine;
using UnityEngine.UI;

public class FoldableElement : MonoBehaviour
{
    [SerializeField] private RectTransform _childParentTransform;
    [SerializeField] private Button _unfoldButton;
    [SerializeField] private Color _highlightColor; 
    private Color _defaultColor;

    private FoldableSubElement _subElement;
    public System.Action<FoldableElement, bool> OnFoldAction;

    // Start is called before the first frame update
    void Start()
    {
        _defaultColor = _unfoldButton.targetGraphic.color;
        _unfoldButton.onClick.AddListener(delegate
        {
            Fold(!_childParentTransform.gameObject.activeSelf); 
            OnFoldAction?.Invoke(this, _childParentTransform.gameObject.activeSelf);
        });
    }

    public void Fold(bool isFolded)
    {
        _childParentTransform.gameObject.SetActive(isFolded);
        _unfoldButton.targetGraphic.color = isFolded ? _defaultColor : _highlightColor;
    }
}
