using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerLobbyElement : MonoBehaviour
{
	[SerializeField] private Button _kickButton;
	[SerializeField] private Image _companyImage;
	[SerializeField] private Button _companyButton;
	[SerializeField] private Text _playerText;
	[SerializeField] private InputField _companyNameInputField;

	// Use this for initialization
	void Start () {
		_kickButton.onClick.AddListener(OnKickButton);
		_companyButton.onClick.AddListener(OnCompanyButton);
		_companyNameInputField.onEndEdit.AddListener(OnCompanyNameEdit);
	}

	private void OnKickButton()
	{
		Debug.Log("Kick");
	}

	private void OnCompanyButton()
	{
		Debug.Log("Change Company");
	}

	private void OnCompanyNameEdit(string value)
	{
		Debug.Log("Company Name Changed");
	}
}
