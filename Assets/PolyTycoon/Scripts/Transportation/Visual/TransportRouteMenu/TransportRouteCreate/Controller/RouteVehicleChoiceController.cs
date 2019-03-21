using Assets.PolyTycoon.Scripts.Transportation.Model.Transport;
using Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.VehicleChoice;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.Controller
{
	public class RouteVehicleChoiceController : MonoBehaviour
	{
		private TransportVehicle _selectedVehicle;
		[SerializeField] private Button _vehicleSelectButton;
		[SerializeField] private GameObject _visibleGameObject;

		[Header("Vehicle Settings")]
		[SerializeField] private RouteVehicleInformationView _routeVehicleInformationUi;
		[SerializeField] private RouteVehicleChoiceView _routeVehicleChoiceUi;

		public TransportVehicle SelectedVehicle {
			get {
				return _selectedVehicle;
			}

			set {
				_selectedVehicle = value;
				SetVehicleChoiceVisible(false);
			}
		}

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
			_vehicleSelectButton.onClick.AddListener(OnVehicleSelectClick);
		}

		private void OnVehicleSelectClick()
		{
			SetVehicleChoiceVisible(true);
		}

		private void SetVehicleChoiceVisible(bool visible)
		{
			_routeVehicleInformationUi.VisibleGameObject.SetActive(!visible);
			_routeVehicleInformationUi.UpdateUI();
			_routeVehicleChoiceUi.VisibleGameObject.SetActive(visible);
		}

		public void Reset()
		{
			SelectedVehicle = null;
			_routeVehicleInformationUi.VisibleGameObject.SetActive(true);
			_routeVehicleChoiceUi.VisibleGameObject.SetActive(false);
			_routeVehicleInformationUi.Reset();
		}
	}
}
