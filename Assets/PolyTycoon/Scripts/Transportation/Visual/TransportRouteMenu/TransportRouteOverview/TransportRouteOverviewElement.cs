using Assets.PolyTycoon.Scripts.Transportation.Model.TransportRoute;
using Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu
{
	public class TransportRouteOverviewElement : PoolableObject {

		private static TransportRouteCreateController _transportRouteCreateController;
		private static TransportRouteManager _transportRouteManager;
		private TransportRoute _transportRoute;

		[Header("Navigation")]
		[SerializeField] private Button _editButton;
		[SerializeField] private Button _removeButton;

		[Header("Information")]
		[SerializeField] private Text _routeNameText;

		public TransportRoute TransportRoute {
			get {
				return _transportRoute;
			}

			set {
				_transportRoute = value;
				_routeNameText.text = _transportRoute.RouteName;
			}
		}

		private void Start()
		{
			_editButton.onClick.AddListener(OnEditClick);
			_removeButton.onClick.AddListener(OnRemoveClick);
		}

		private void OnEditClick()
		{
			if (!_transportRouteCreateController) _transportRouteCreateController = FindObjectOfType<TransportRouteCreateController>();
			_transportRouteCreateController.LoadTransportRoute(TransportRoute);
		}

		private void OnRemoveClick()
		{
			if (!_transportRouteManager) _transportRouteManager = FindObjectOfType<TransportRouteManager>();
			_transportRouteManager.RemoveTransportRoute(TransportRoute);
		}

		private void Reset()
		{
			_routeNameText.text = "";
			_transportRoute = null;
		}

		public override void Hide()
		{
			base.Hide();
			Reset();
		}
	}
}
