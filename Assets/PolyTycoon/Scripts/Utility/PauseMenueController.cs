using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenueController : AbstractUi
{
	[SerializeField] private Button _resumeButton;
	[SerializeField] private Button _settingsButton;
	[SerializeField] private Button _exitButton;

	// Use this for initialization
	void Start () {
		_resumeButton.onClick.AddListener(OnResumeClick);
		_settingsButton.onClick.AddListener(OnSettingsClick);
		_exitButton.onClick.AddListener(OnExitClick);
	}

	private void OnResumeClick()
	{
		SetVisible(false);
	}

	private void OnSettingsClick()
	{
		Debug.Log("Setting View");
	}

	private void OnExitClick()
	{
		Debug.Log("Exit Click");
		Application.Quit();
		//SceneController sceneController = FindObjectOfType<SceneController>();
		//sceneController.LoadScene(SceneController.Scenes.MainMenu);
	}

	public override void OnShortCut()
	{
		base.OnShortCut();
		SetVisible(!VisibleObject.activeSelf);
	}
}
