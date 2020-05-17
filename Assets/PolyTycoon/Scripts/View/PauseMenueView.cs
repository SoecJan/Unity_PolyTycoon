using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenueView : AbstractUi
{
	public static System.Action<bool> _onActivation;
	private bool _isVisible = false; // Workaround for animation and VisibleObject.activeSelf conflict
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
		_isVisible = false;
		SetVisible(_isVisible);
		_onActivation?.Invoke(_isVisible);
		Time.timeScale = 1f;
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
		_isVisible = !_isVisible;
		SetVisible(_isVisible);
		_onActivation?.Invoke(_isVisible);
		if (_isVisible) Time.timeScale = 0f;
	}
}
