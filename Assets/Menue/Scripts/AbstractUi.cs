using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractUi : MonoBehaviour
{
	[SerializeField] private GameObject _visibleObject;

	public GameObject VisibleObject {
		get {
			return _visibleObject;
		}

		set {
			_visibleObject = value;
		}
	}

	public void SetVisible(bool visible)
	{
		VisibleObject.SetActive(visible);
		OnVisibilityChange(visible);
	}

	public abstract void Reset();

	protected abstract void OnVisibilityChange(bool visible);
}
