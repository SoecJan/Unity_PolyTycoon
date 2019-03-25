using System.Collections.Generic;
using Assets.PolyTycoon.Scripts.Transportation.Model.Transport;
using Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.Controller;
using Assets.PolyTycoon.Scripts.Transportation.Visual.Vehicle;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;

namespace Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.VehicleChoice
{
	public class RouteVehicleChoiceView : MonoBehaviour
	{
		private static RouteVehicleChoiceController _routeVehicleChoiceController;
		private static VehicleManager _vehicleManager;

		[Header("General")]
		[SerializeField] private GameObject _visibleGameObject;

		[Header("UI")]
		[SerializeField] private ScrollViewHandle _vehicleChoiceScrollView;
		[SerializeField] private GameObject _vehicleChoiceElementPrefab;

		public GameObject VisibleGameObject {
			get {
				return _visibleGameObject;
			}

			set {
				_visibleGameObject = value;
			}
		}

		private void Start()
		{
			if (!_routeVehicleChoiceController) _routeVehicleChoiceController = FindObjectOfType<RouteVehicleChoiceController>();
			VehicleView.OnClickAction += view =>
			{
				_routeVehicleChoiceController.SelectedVehicle = (TransportVehicle) view.Vehicle;
				VisibleGameObject.SetActive(false);
			};
			FillView();
		}

		public void OnAllFilterClick()
		{
			Debug.Log("All Filter");
		}

		public void OnTruckFilterClick()
		{
			Debug.Log("Truck Filter");
		}

		public void OnTrainFilterClick()
		{
			Debug.Log("Train Filter");
		}

		public void OnShipFilterClick()
		{
			Debug.Log("Ship Filter");
		}

		public void OnPlaneFilterClick()
		{
			Debug.Log("Plane Filter");
		}

		internal void FillView()
		{
			if (!_vehicleManager)
			{
				_vehicleManager = FindObjectOfType<VehicleManager>();
			}
			foreach (Model.Transport.Vehicle vehicle in _vehicleManager.VehicleList)
			{
				if (!(vehicle is TransportVehicle)) continue;
				GameObject vehicleViewObject = _vehicleChoiceScrollView.AddObject((RectTransform)_vehicleChoiceElementPrefab.transform);
				VehicleView vehicleView = vehicleViewObject.GetComponent<VehicleView>();
				if (vehicleView)
					vehicleView.Vehicle = vehicle;
			}
		}
	}
}
