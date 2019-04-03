using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{

	#region Attributes

	private TransportVehicleUi _transportVehicleUi;
	[SerializeField] private List<Vehicle> _vehicleList;
	#endregion

	#region Getter & Setter
	public List<Vehicle> VehicleList {
		get {
			return _vehicleList;
		}

		set {
			_vehicleList = value;
		}
	}

	public List<GameObject> InstancedVehicleList { get; set; }
	#endregion

	#region Methods
	// Use this for initialization
	void Start()
	{
		_transportVehicleUi = FindObjectOfType<TransportVehicleUi>();
		InstancedVehicleList = new List<GameObject>();
	}

	public GameObject AddVehicle(Vehicle vehicle, Vector3 position, Vector3 eulerAngle = default(Vector3))
	{
		GameObject vehicleGameObject = Instantiate(vehicle.gameObject, position, Quaternion.Euler(eulerAngle));
		TransportVehicle transportVehicle = vehicleGameObject.GetComponent<TransportVehicle>();
		if (transportVehicle)
		{
			transportVehicle.OnClickAction -= OnVehicleClick;
			transportVehicle.OnClickAction += OnVehicleClick;
		}
		InstancedVehicleList.Add(vehicleGameObject);
		vehicleGameObject.transform.parent = transform;
		return vehicleGameObject;
	}

	private void OnVehicleClick(TransportVehicle transportVehicle)
	{
		_transportVehicleUi.DisplayedTransportVehicle = transportVehicle;
	}
	#endregion
}