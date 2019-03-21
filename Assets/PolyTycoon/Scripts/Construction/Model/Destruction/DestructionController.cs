using System.Collections;
using System.Collections.Generic;
using Assets.PolyTycoon.Scripts.Construction.Model.Placement;
using UnityEngine;

public class DestructionController : MonoBehaviour
{
	private static bool _destructionActive;
	private Camera _mainCamera;
	private BuildingManager _buildingManager;
	[SerializeField] private LayerMask _buildingMask;

	public static bool DestructionActive {
		get {
			return _destructionActive;
		}

		set {
			_destructionActive = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
		_buildingManager = FindObjectOfType<GroundPlacementController>().BuildingManager;
		_mainCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		if (!_destructionActive || !Input.GetMouseButtonDown(0)) return;

		Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _buildingMask)) return;

		SimpleMapPlaceable mapPlaceable = hitInfo.collider.gameObject.GetComponent<SimpleMapPlaceable>();
		if (!mapPlaceable) return;

		_buildingManager.RemoveMapPlaceable(mapPlaceable.transform.position);
	}
}
