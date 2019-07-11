using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldToScreenUiManager : MonoBehaviour
{

	[SerializeField] private Button _vibilityButton;
	[SerializeField] private GameObject _visibleGameObject;

	private Camera _mainCamera;
	private List<WorldUiElement> _handledTransforms;

	void Start()
	{
		_vibilityButton.onClick.AddListener(delegate { _visibleGameObject.SetActive(!_visibleGameObject.activeSelf); });
		_handledTransforms = new List<WorldUiElement>();
		_mainCamera = Camera.main;
	}

	void LateUpdate()
	{
		foreach (WorldUiElement worldUiElement in _handledTransforms)
		{
			Vector3 position = _mainCamera.WorldToScreenPoint(worldUiElement.AnchorTransform.position);
			worldUiElement.UiTransform.position = position + worldUiElement.Offset;
		}
	}

	public WorldUiElement Add(GameObject uiPrefab, Transform anchorTransform, Vector3 offset)
	{
		GameObject instanceObject = Instantiate(uiPrefab, _visibleGameObject.transform);
		WorldUiElement uiElement = new WorldUiElement(instanceObject.transform, anchorTransform, offset);
		_handledTransforms.Add(uiElement);
		return uiElement;
	}

	public void Remove(WorldUiElement worldUiObject)
	{
		_handledTransforms.Remove(worldUiObject);
		Destroy(worldUiObject.UiTransform.gameObject);
		Destroy(worldUiObject.AnchorTransform.gameObject);
	}

	public struct WorldUiElement
	{
		private Transform _uiTransform;
		private Transform _anchorTransform;
		private Vector3 _offset;

		public WorldUiElement(Transform uiTransform, Transform anchorTransform) : this(uiTransform, anchorTransform, Vector3.zero)
		{}

		public WorldUiElement(Transform uiTransform, Transform anchorTransform, Vector3 offset)
		{
			_uiTransform = uiTransform;
			_anchorTransform = anchorTransform;
			_offset = offset;
		}

		public Transform AnchorTransform {
			get {
				return _anchorTransform;
			}

			set {
				_anchorTransform = value;
			}
		}

		public Transform UiTransform {
			get {
				return _uiTransform;
			}

			set {
				_uiTransform = value;
			}
		}

		public Vector3 Offset {
			get {
				return _offset;
			}

			set {
				_offset = value;
			}
		}
	}
}
