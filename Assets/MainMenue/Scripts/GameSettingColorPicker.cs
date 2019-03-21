using System.Collections;
using System.Collections.Generic;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Image = UnityEngine.UI.Image;

public class GameSettingColorPicker : MonoBehaviour
{

	[SerializeField] private Color[] _availableColors;
	[SerializeField] private GameObject _visibleGameObject;

	[SerializeField] private ScrollViewHandle _scrollView;
	[SerializeField] private GameSettingColorPickerElement _colorPrefabImage;

	public GameObject VisibleGameObject {
		get {
			return _visibleGameObject;
		}

		set {
			_visibleGameObject = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
		foreach (Color color in _availableColors)
		{
			GameObject colorImageGameObject = _scrollView.AddObject((RectTransform) _colorPrefabImage.transform);
			GameSettingColorPickerElement colorPickerElement = colorImageGameObject.GetComponent<GameSettingColorPickerElement>();
			colorPickerElement.Image.color = color;
		}
	}
}
