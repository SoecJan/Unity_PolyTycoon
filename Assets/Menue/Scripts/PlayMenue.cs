using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayMenue : AbstractUi
{

	[SerializeField] private Button _singleplayerButton;
	[SerializeField] private Button _multiplayerButton;
	[SerializeField] private Button _backButton;

	private MainMenue _mainMenue;
	
	void Start ()
	{
		_mainMenue = FindObjectOfType<MainMenue>();
		_singleplayerButton.onClick.AddListener(OnSingleplayerClick);
		_multiplayerButton.onClick.AddListener(OnMultiplayerClick);
		_backButton.onClick.AddListener(OnBackClick);
	}

	private void OnSingleplayerClick()
	{
		Debug.Log("SinglePlayer");
	}

	private void OnMultiplayerClick()
	{
		Debug.Log("Multiplayer");
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
