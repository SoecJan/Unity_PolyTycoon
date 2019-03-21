using System;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerLobbyController : MonoBehaviour
{
	[SerializeField] private MultiplayerSettingView _multiplayerSettingView;
	[SerializeField] private MultiplayerPlayerView _multiplayerPlayerView;

	[SerializeField] private Button _playerViewButton;
	[SerializeField] private Button _settingViewButton;
	[SerializeField] private Button _leaveButton;
	[SerializeField] private Button _readyButton;

	void Start()
	{
		_playerViewButton.onClick.AddListener(OnPlayerViewClick);
		_settingViewButton.onClick.AddListener(OnSettingClick);
		_leaveButton.onClick.AddListener(OnLeaveClick);
		_readyButton.onClick.AddListener(OnReadyClick);
	}

	private void OnReadyClick()
	{
		Debug.Log("Ready");
	}

	private void OnLeaveClick()
	{
		Debug.Log("Leave");
	}

	private void OnSettingClick()
	{
		_multiplayerPlayerView.VisibleGameObject.SetActive(false);
		_multiplayerSettingView.VisibleGameObject.SetActive(true);
	}

	private void OnPlayerViewClick()
	{
		_multiplayerSettingView.VisibleGameObject.SetActive(false);
		_multiplayerPlayerView.VisibleGameObject.SetActive(true);
	}
}

[Serializable]
public class MultiplayerSettingView
{
	[SerializeField] private GameObject _visibleGameObject;
	[SerializeField] private InputField _startMoneyInputField;
	[SerializeField] private InputField _seedInputField;
	[SerializeField] private Slider _cityCountSlider;
	[SerializeField] private Slider _mapSizeSlider;

	public GameObject VisibleGameObject {
		get {
			return _visibleGameObject;
		}

		set {
			_visibleGameObject = value;
		}
	}
}

[Serializable]
public class MultiplayerPlayerView
{
	[SerializeField] private GameObject _visibleGameObject;
	[SerializeField] private ScrollViewHandle _scrollView;
	[SerializeField] private MultiplayerLobbyElement _scrollViewElement;

	public GameObject VisibleGameObject {
		get {
			return _visibleGameObject;
		}

		set {
			_visibleGameObject = value;
		}
	}
}