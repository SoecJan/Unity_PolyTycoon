using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsUi : AbstractUi
{

	[SerializeField] private Button _showButton;

	public override void OnShortCut()
	{
		base.OnShortCut();
		SetVisible(!VisibleObject.activeSelf);
	}

	// Use this for initialization
	void Start () {
		_showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf); });
	}
}
