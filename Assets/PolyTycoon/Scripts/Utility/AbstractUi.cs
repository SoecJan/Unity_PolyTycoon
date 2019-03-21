using UnityEngine;

public class AbstractUiiOld : MonoBehaviour
{
	protected static AbstractUiiOld _visibleUi;

	[SerializeField] private GameObject _visibleGameObject;

	public GameObject VisibleGameObject {
		get {
			return _visibleGameObject ? _visibleGameObject : gameObject;
		}
	}

	public virtual void OnShortCut() {}

	public virtual void Reset() {}

	public void SetVisible(bool visible)
	{
		if (visible)
		{
			if (_visibleUi) _visibleUi.VisibleGameObject.SetActive(false);
			_visibleUi = this;
		}
		else
		{
			Reset();
			_visibleUi = null;
		}
		VisibleGameObject.SetActive(visible);
	}

	void Update()
	{
		if (!VisibleGameObject.activeSelf || Input.GetAxis("Cancel") <= 0) return;
		SetVisible(false);
	}
}
