using Assets.PolyTycoon.Scripts.Transportation.Model.TransportRoute;
using Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate
{
	public class TransportRouteCreateController : AbstractUi
	{
		[Header("Navigation")]
		[SerializeField] private Button _createRouteButton;
		[SerializeField] private Button _showButton;
		[SerializeField] private Button _exitButton;
		[Header("Controller")]
		[SerializeField] private TransportRouteManager _transportRouteManager;
		[SerializeField] private RouteVehicleChoiceController _routeVehicleChoiceController;
		[SerializeField] private RouteSettingController _settingController;
		[SerializeField] private RouteElementController _routeElementController;

		public RouteSettingController SettingController {
			get {
				return _settingController;
			}

			set {
				_settingController = value;
			}
		}

		public RouteElementController RouteElementController {
			get {
				return _routeElementController;
			}

			set {
				_routeElementController = value;
			}
		}

		public RouteVehicleChoiceController RouteVehicleChoiceController {
			get {
				return _routeVehicleChoiceController;
			}

			set {
				_routeVehicleChoiceController = value;
			}
		}

		private void Start()
		{
			_createRouteButton.onClick.AddListener(_transportRouteManager.OnTransportRouteCreate);
			_showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf); });
			_exitButton.onClick.AddListener(delegate { SetVisible(false); });
		}

		public void LoadTransportRoute(TransportRoute transportRoute)
		{
			SetVisible(true);
			RouteVehicleChoiceController.SelectedVehicle = transportRoute.Vehicle;
			RouteElementController.LoadTransportRoute(transportRoute);
		}

		public new void Reset()
		{
			_routeVehicleChoiceController.Reset();
			_settingController.Reset();
			_routeElementController.Reset();
		}

		public override void OnShortCut()
		{
			SetVisible(true);
		}
	}
}
