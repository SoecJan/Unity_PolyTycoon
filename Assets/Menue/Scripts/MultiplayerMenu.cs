using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenu : AbstractUi
{

	[Header("Navigation")] 
	[SerializeField] private Button _backButton;

	private PlayMenue _playMenue;

	// Use this for initialization
	void Start () {
		_backButton.onClick.AddListener(OnBackClick);
		_playMenue = FindObjectOfType<PlayMenue>();
	}

	private void OnBackClick()
	{
		SetVisible(false);
		_playMenue.SetVisible(true);
	}

	public override void Reset()
	{}

	protected override void OnVisibilityChange(bool visible)
	{}
}
