using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IUserNotificationView
{
	string InformationText { set; }
}

/// <summary>
/// Displays information to the user.
/// </summary>
public class UserNotificationView : MonoBehaviour, IUserNotificationView
{
	#region Attributes
	[SerializeField] private GameObject _visibleGameObject;
	[SerializeField] private float _displayTime;
	[SerializeField] private TextMeshProUGUI _informationText;
	[SerializeField] private Button _exitButton;
	private Coroutine _coroutine;
	#endregion

	#region Methods
	private void Start()
	{
		_exitButton.onClick.AddListener(Reset);
	}

	public string InformationText {
		set {
			_informationText.text = value;
			
			if (_coroutine != null) StopCoroutine(_coroutine);
			_coroutine = StartCoroutine(DisplayInformation());
		}
	}

	private IEnumerator DisplayInformation()
	{
		_visibleGameObject.SetActive(true);
		yield return new WaitForSeconds(_displayTime);
		Reset();
	}

	private void Reset()
	{
		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
			_coroutine = null;
		}
		_visibleGameObject.SetActive(false);
		_informationText.text = "";
		
	}
	#endregion
}

