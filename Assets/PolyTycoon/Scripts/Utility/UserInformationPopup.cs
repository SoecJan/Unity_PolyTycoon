using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class UserInformationPopup : MonoBehaviour
{
	#region Attributes
	[SerializeField] private Text _informationText;
	[SerializeField] private GameObject _visibleGameObject;
	[SerializeField] private float _displayTime;
	[SerializeField] private Button _exitButton;
	private Coroutine _coroutine;
	float _activeTime = 0f;
	#endregion

	#region Methods
	private void Start()
	{
		_exitButton.onClick.AddListener(OnExitClick);
	}

	public string InformationText {
		set {
			_informationText.text = value;
			if (_activeTime > _displayTime || _activeTime == 0f)
			{
				_coroutine = StartCoroutine(DisplayInformation());
			}
			_activeTime = 0f;
		}
	}

	private IEnumerator DisplayInformation()
	{
		_visibleGameObject.SetActive(true);
		while (_activeTime < _displayTime)
		{
			_activeTime += Time.deltaTime;
			yield return 1;
		}
		OnExitClick();
		_coroutine = null;
		yield return null;
	}

	private void OnExitClick()
	{
		if (_coroutine != null)
			StopCoroutine(_coroutine);
		_visibleGameObject.SetActive(false);
		_informationText.text = "";
		_activeTime = 0f;
	}
	#endregion
}

