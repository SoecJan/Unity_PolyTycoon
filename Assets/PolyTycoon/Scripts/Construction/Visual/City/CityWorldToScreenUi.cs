using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityWorldToScreenUi : MonoBehaviour
{
	[SerializeField] private Text _text;
	[SerializeField] private GameObject _visibleGameObject;

	public Text Text {
		get {
			return _text;
		}
	}
}
