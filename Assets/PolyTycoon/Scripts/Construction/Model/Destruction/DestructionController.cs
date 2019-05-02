using UnityEngine;

public class DestructionController : MonoBehaviour
{
	private Camera _mainCamera;
	private BuildingManager _buildingManager;
	[SerializeField] private LayerMask _buildingMask;

	public static bool DestructionActive { get; set; }

	// Use this for initialization
	void Start ()
	{
		_buildingManager = FindObjectOfType<GroundPlacementController>().BuildingManager;
		_mainCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		if (!DestructionActive || !Input.GetMouseButtonDown(0)) return;

		Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _buildingMask)) return;

		SimpleMapPlaceable mapPlaceable = hitInfo.collider.gameObject.GetComponent<SimpleMapPlaceable>();
		ComplexMapPlaceable complexMapPlaceable = mapPlaceable as ComplexMapPlaceable;
		if (!mapPlaceable) return;
		if (complexMapPlaceable)
		{
			foreach (SimpleMapPlaceable childMapPlaceable in complexMapPlaceable.ChildMapPlaceables)
			{
				_buildingManager.RemoveMapPlaceable(childMapPlaceable);
			}
			Destroy(complexMapPlaceable.gameObject);
		}
		else
		{
			_buildingManager.RemoveMapPlaceable(mapPlaceable.transform.position);
		}
		
	}
}
