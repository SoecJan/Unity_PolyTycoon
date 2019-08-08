using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityWorldToScreenUi : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _text;
	[SerializeField] private GameObject _visibleGameObject;

	public string Text {
		get {
			return _text.text;
		}
		set { _text.text = value; }
	}
}
