using System.Collections;
using System.Collections.Generic;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerBrowserController : AbstractUi
{

	[SerializeField] private ScrollViewHandle _scrollView;
	[SerializeField] private GameObject _scrollViewElement;

	[SerializeField] private Button _joinButton;
	[SerializeField] private Button _backButton;

	private PlayMenueController _playMenueController;

	void Start()
	{
		_playMenueController = FindObjectOfType<PlayMenueController>();
		_backButton.onClick.AddListener(OnBackClick);
		_joinButton.onClick.AddListener(OnJoinClick);
	}

	private void OnBackClick()
	{
		SetVisible(false);
		_playMenueController.SetVisible(true);
	}

	private void OnJoinClick()
	{
		Debug.Log("Join");
	}
}
