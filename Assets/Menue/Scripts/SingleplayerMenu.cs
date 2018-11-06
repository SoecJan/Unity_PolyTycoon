using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleplayerMenu : AbstractUi 
{
	[Header("Navigation")]
	[SerializeField] private Button _backButton;

	private PlayMenue _playMenu;

	// Use this for initialization
	void Start () {
		_playMenu = FindObjectOfType<PlayMenue>();
		_backButton.onClick.AddListener(OnBackClick);
	}

	private void OnBackClick()
	{
		SetVisible(false);
		_playMenu.SetVisible(true);
	}

	public override void Reset()
	{}

	protected override void OnVisibilityChange(bool visible)
	{}
}
