using UnityEngine;
using UnityEngine.UI;

public class SettingsMenueController : AbstractUi
{

	[SerializeField] private Slider _musicVolumeSlider;
	[SerializeField] private Slider _effectVolumeSlider;
	[SerializeField] private Button _backButton;

	private MainMenueController _mainMenueController;

	// Use this for initialization
	void Start ()
	{
		_mainMenueController = FindObjectOfType<MainMenueController>();
		_backButton.onClick.AddListener(OnBackClick);
	}

	private void OnBackClick()
	{
		SetVisible(false);
		_mainMenueController.SetVisible(true);
	}
}
