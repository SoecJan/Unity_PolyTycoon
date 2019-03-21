using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayMenueController : AbstractUi
{

	[SerializeField] private Button _singleplayerButton;
	[SerializeField] private Button _multiplayerButton;
	[SerializeField] private Button _backButton;

	private MainMenueController _mainMenueController;
	private GameSettingController _gameSettingController;
	private MultiplayerBrowserController _multiplayerBrowserController;

	// Use this for initialization
	void Start ()
	{
		_gameSettingController = FindObjectOfType<GameSettingController>();
		_mainMenueController = FindObjectOfType<MainMenueController>();
		_multiplayerBrowserController = FindObjectOfType<MultiplayerBrowserController>();
		_singleplayerButton.onClick.AddListener(OnSingleplayerButtonClick);
		_multiplayerButton.onClick.AddListener(OnMultiplayerButtonClick);
		_backButton.onClick.AddListener(OnBackButtonClick);
	}

	private void OnSingleplayerButtonClick()
	{
		SetVisible(false);
		_gameSettingController.SetVisible(true);
	}

	private void OnMultiplayerButtonClick()
	{
		SetVisible(false);
		_multiplayerBrowserController.SetVisible(true);
	}

	private void OnBackButtonClick()
	{
		SetVisible(false);
		_mainMenueController.SetVisible(true);

	}
}
