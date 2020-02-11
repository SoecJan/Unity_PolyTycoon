using UnityEngine;

public struct WorldToScreenElement
{
    private Transform _uiTransform;
    private Transform _anchorTransform;
    private Vector3 _offset;

    public WorldToScreenElement(Transform uiTransform, Transform anchorTransform) : this(uiTransform, anchorTransform,
        Vector3.zero)
    { }

    public WorldToScreenElement(Transform uiTransform, Transform anchorTransform, Vector3 offset)
    {
        _uiTransform = uiTransform;
        _anchorTransform = anchorTransform;
        _offset = offset;
    }

    public Transform AnchorTransform
    {
        get { return _anchorTransform; }

        set { _anchorTransform = value; }
    }

    public Transform UiTransform
    {
        get { return _uiTransform; }

        set { _uiTransform = value; }
    }

    public Vector3 Offset
    {
        get { return _offset; }

        set { _offset = value; }
    }
}