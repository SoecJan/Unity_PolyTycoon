using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayMenue : AbstractUi
{
	[Header("Navigation")]
	[SerializeField] private Button _singleplayerButton;
	[SerializeField] private Button _multiplayerButton;
	[SerializeField] private Button _backButton;

	private SingleplayerMenu _singleplayerMenu;
	private MultiplayerMenu _multiplayerMenu;
	private MainMenue _mainMenue;
	
	void Start ()
	{
		_mainMenue = FindObjectOfType<MainMenue>();
		_singleplayerMenu = FindObjectOfType<SingleplayerMenu>();
		_multiplayerMenu = FindObjectOfType<MultiplayerMenu>();
		_singleplayerButton.onClick.AddListener(OnSingleplayerClick);
		_multiplayerButton.onClick.AddListener(OnMultiplayerClick);
		_backButton.onClick.AddListener(OnBackClick);
	}

	private void OnSingleplayerClick()
	{
		SetVisible(false);
		_singleplayerMenu.SetVisible(true);
	}

	private void OnMultiplayerClick()
	{
		SetVisible(false);
		_multiplayerMenu.SetVisible(true);
	}

	private void OnBackClick()
	{
		SetVisible(false);
		_mainMenue.SetVisible(true);
	}

	public override void Reset()
	{}

	protected override void OnVisibilityChange(bool visible)
	{}
}
