using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldToScreenManager : MonoBehaviour, IWorldToScreenManager
{
	[SerializeField] private Button _vibilityButton;
	[SerializeField] private GameObject _visibleGameObject;

	private Camera _mainCamera;
	private List<WorldToScreenElement> _handledTransforms;

	void Start()
	{
		_vibilityButton.onClick.AddListener(delegate { _visibleGameObject.SetActive(!_visibleGameObject.activeSelf); });
		_handledTransforms = new List<WorldToScreenElement>();
		_mainCamera = Camera.main;
	}

	void LateUpdate()
	{
		foreach (WorldToScreenElement worldUiElement in _handledTransforms)
		{
			Vector3 position = _mainCamera.WorldToScreenPoint(worldUiElement.AnchorTransform.position);
			worldUiElement.UiTransform.position = position + worldUiElement.Offset;
		}
	}

	public WorldToScreenElement Add(GameObject uiPrefab, Transform anchorTransform, Vector3 offset)
	{
		GameObject instanceObject = Instantiate(uiPrefab, _visibleGameObject.transform);
		WorldToScreenElement uiElement = new WorldToScreenElement(instanceObject.transform, anchorTransform, offset);
		_handledTransforms.Add(uiElement);
		return uiElement;
	}

	public void Remove(WorldToScreenElement worldUiObject)
	{
		_handledTransforms.Remove(worldUiObject);
		Destroy(worldUiObject.UiTransform.gameObject);
		Destroy(worldUiObject.AnchorTransform.gameObject);
	}
}
