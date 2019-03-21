using System;
using Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PolyTycoon.Scripts.Transportation.Visual.Vehicle
{
	public class VehicleView : MonoBehaviour
	{
		#region Attributes

		private static System.Action<VehicleView> _onClickAction;
		private static RouteVehicleChoiceController _routeVehicleChoiceController;
		private Model.Transport.Vehicle _vehicle;

		[SerializeField] private Button _selectButton;
		[SerializeField] private Image _image;

		void Start()
		{
			_selectButton.onClick.AddListener(delegate{
				OnClickAction(this);
			});
		}

		public static Action<VehicleView> OnClickAction {
			get {
				return _onClickAction;
			}

			set {
				_onClickAction = value;
			}
		}
		#endregion

		#region Getter & Setter
		public Model.Transport.Vehicle Vehicle {
			get {
				return _vehicle;
			}

			set {
				_vehicle = value;
				this._image.sprite = _vehicle.Sprite;
			}
		}
		#endregion

		#region Methods

		#endregion
	}
}
