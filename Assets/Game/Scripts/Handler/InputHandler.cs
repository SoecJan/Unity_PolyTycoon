using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
	private List<InputListener> _inputListeners;

	// Start is called before the first frame update
	void Start()
	{
		_inputListeners = new List<InputListener> {new InteractableObjectInputListener()};
	}

	// Update is called once per frame
	void Update()
	{
		if (EventSystem.current.IsPointerOverGameObject()) return;
		if (!Input.GetMouseButtonDown(0)) return;

		//  RaycastHit hit;
		//  if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, LayerMask.NameToLayer("Terrain"))) {}

		foreach (InputListener listener in _inputListeners)
		{
			if (listener.HandleInput()) break;
		}
	}
}

public abstract class InputListener
{
	public abstract bool HandleInput();
}

class InteractableObjectInputListener : InputListener
{
	public override bool HandleInput()
	{
		RaycastHit hit;
		if (!Physics.Raycast(
			Camera.main.ScreenPointToRay(Input.mousePosition), 
			out hit, 
			float.MaxValue,
			LayerMask.NameToLayer("Factory"))) 
				return false;

		InteractableObject hitObject = hit.collider.gameObject.GetComponent<InteractableObject>();
		if (!hitObject) return false;

		// Open Object Information UI
		hitObject.ShowUI();

		return true;
	}
}

class PlacementInputListener : InputListener
{
	private Dictionary<Vector3Int, PlacedObject> _placedObjects;
	private PlacedObject _objectToPlace;

	public PlacementInputListener()
	{
		_placedObjects = new Dictionary<Vector3Int, PlacedObject>();
	}

	public override bool HandleInput()
	{
		RaycastHit hit;
		if (!Physics.Raycast(
			Camera.main.ScreenPointToRay(Input.mousePosition),
			out hit,
			float.MaxValue,
			LayerMask.NameToLayer("Terrain")))
			return false;

		AddObject(_objectToPlace, hit.point);
		return true;
	}

	public void AddObject(PlacedObject interactableObject, Vector3 position)
	{
		Vector3Int normalizedVector3 = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);
		if (_placedObjects.ContainsKey(normalizedVector3)) return;

		_placedObjects.Add(normalizedVector3, interactableObject);

		_objectToPlace = null;
	}
}