using UnityEngine;
using UnityEngine.UI;

public class MainMenueController : AbstractUi
{

	[SerializeField] private Button _playButton;
	[SerializeField] private Button _optionsButton;
	[SerializeField] private Button _exitButton;

	private PlayMenueController _playMenueController;
	private SettingsMenueController _settingsMenueController;

	void Start ()
	{
		_playMenueController = FindObjectOfType<PlayMenueController>();
		_settingsMenueController = FindObjectOfType<SettingsMenueController>();
		_playButton.onClick.AddListener(OnPlayClick);
		_optionsButton.onClick.AddListener(OnOptionsClick);
		_exitButton.onClick.AddListener(OnExitClick);
	}

	private void OnPlayClick()
	{
		SetVisible(false);
		_playMenueController.SetVisible(true);
	}

	private void OnOptionsClick()
	{
		SetVisible(false);
		_settingsMenueController.SetVisible(true);
	}

	private void OnExitClick()
	{
		Application.Quit();
	}
}
