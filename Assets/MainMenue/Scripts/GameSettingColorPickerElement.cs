using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingColorPickerElement : MonoBehaviour
{
	private static System.Action<GameSettingColorPickerElement> _onClickAction;
	[SerializeField] private Image _image;
	[SerializeField] private Button _clickButton;

	void Start()
	{
		_clickButton.onClick.AddListener(OnClick);
	}

	public static Action<GameSettingColorPickerElement> OnClickAction {
		get {
			return _onClickAction;
		}

		set {
			_onClickAction = value;
		}
	}

	public Image Image {
		get {
			return _image;
		}

		set {
			_image = value;
		}
	}

	private void OnClick()
	{
		OnClickAction(this);
	}
}
