using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenue : AbstractUi
{
	[Header("UI")]
	[SerializeField] private Button _playButton;
	[SerializeField] private Button _optionsButton;
	[SerializeField] private Button _exitButton;

	[Header("References")] 
	[SerializeField] private AbstractUi _playUi;
	[SerializeField] private AbstractUi _optionsUi;

	// Use this for initialization
	void Start () {
		_playButton.onClick.AddListener(OnPlayClick);
		_optionsButton.onClick.AddListener(OnOptionsClick);
		_exitButton.onClick.AddListener(Application.Quit);
	}

	private void OnPlayClick()
	{
		SetVisible(false);
		_playUi.SetVisible(true);
	}

	private void OnOptionsClick()
	{
		SetVisible(false);
		_optionsUi.SetVisible(true);
	}

	public new void Reset()
	{}

	protected new void OnVisibilityChange(bool visible)
	{}
}
