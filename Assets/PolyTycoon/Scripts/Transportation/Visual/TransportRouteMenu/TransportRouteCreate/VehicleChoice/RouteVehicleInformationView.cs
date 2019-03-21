using Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.VehicleChoice
{
	public class RouteVehicleInformationView : MonoBehaviour {
		private RouteVehicleChoiceController _routeVehicleChoiceController;

		[Header("General")]
		[SerializeField] private GameObject _visibleGameObject;

		[Header("UI")]
		[SerializeField] private Image _image;
		[SerializeField] private Text _speedText;
		[SerializeField] private Text _capacityText;
		[SerializeField] private Text _unloadSpeedText;

		public GameObject VisibleGameObject {
			get {
				return _visibleGameObject;
			}

			set {
				_visibleGameObject = value;
			}
		}

		private void Awake()
		{
			_routeVehicleChoiceController = FindObjectOfType<RouteVehicleChoiceController>();
		}

		public void UpdateUI()
		{
			if (!_routeVehicleChoiceController || !_routeVehicleChoiceController.SelectedVehicle) return;

			_image.sprite = _routeVehicleChoiceController.SelectedVehicle.Sprite;
			_speedText.text = "" + _routeVehicleChoiceController.SelectedVehicle.Mover.MaxSpeed;
			_capacityText.text = "" + _routeVehicleChoiceController.SelectedVehicle.TotalCapacity;
			_unloadSpeedText.text = "" + _routeVehicleChoiceController.SelectedVehicle.UnloadSpeed;
		}

		public void Reset()
		{
			_image.sprite = null;
			_speedText.text = "Speed";
			_capacityText.text = "Capacity";
			_unloadSpeedText.text = "Unload";
		}
	}
}
