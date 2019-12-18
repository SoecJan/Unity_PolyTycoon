using System;
using System.Collections.Generic;
using UnityEngine;

public class ShortCutManager : MonoBehaviour
{

	[SerializeField] private List<ShortCutTrigger> _shortCutTriggers;

	private void Start()
	{
		PauseMenueController._onActivation += delegate(bool value) { enabled = !value; };
		InputFieldUtility._onInputFieldSelect += delegate { enabled = false; };
		InputFieldUtility._onInputFieldDeselect += delegate { enabled = true; };
	}

	void Update()
	{
		foreach (ShortCutTrigger shortCutTrigger in _shortCutTriggers)
		{
			if (Input.GetKeyDown(shortCutTrigger.KeyCode))
			{
				shortCutTrigger.AbstractUi.OnShortCut();
			}
		}
	}

	[Serializable]
	private struct ShortCutTrigger
	{
		[SerializeField] private KeyCode _keyCode;
		[SerializeField] private AbstractUi _abstractUi;

		public KeyCode KeyCode {
			get {
				return _keyCode;
			}

			set {
				_keyCode = value;
			}
		}

		public AbstractUi AbstractUi {
			get {
				return _abstractUi;
			}

			set {
				_abstractUi = value;
			}
		}
	}
}