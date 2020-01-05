using TMPro;
using UnityEngine;

public class TooltipText : TooltipHandle
{
    private TMP_Text _text;

    public string Text
    {
        get => _text.text;
        set
        {
            if (!_text)
            {
                GameObject _tipPrefab = (GameObject)Resources.Load(PathUtil.Get("ToolTipText"), typeof(GameObject));
                GameObject tip = Instantiate(_tipPrefab, GameObject.Find("TooltipParent").transform);
                _text = tip.GetComponentInChildren<TMP_Text>();
                TipTransform = (RectTransform) tip.transform;
                _offset = Vector3.up * 3;
            }
            _text.text = value;
        }
    }
}
