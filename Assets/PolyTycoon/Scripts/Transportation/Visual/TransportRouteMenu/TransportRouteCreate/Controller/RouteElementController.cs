using System.Collections.Generic;
using Assets.PolyTycoon.Scripts.Construction.Model.Factory;
using Assets.PolyTycoon.Scripts.Transportation.Model.TransportRoute;
using Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.RouteElement;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.Controller
{
	public class RouteElementController : MonoBehaviour
	{
		private List<RouteElementView> _transportRouteElements;
		private RouteElementView _selectedRouteElement;
		[Header("General")]
		[SerializeField] private InputField _routeNameField;
		[SerializeField] private GameObject _visibleGameObject;

		[Header("Route Element UI")]
		[SerializeField] private ScrollViewHandle _routeElementScrollView;
		[SerializeField] private GameObject _routeElementPrefab;
		[SerializeField] private UserInformationPopup _userInformationPopup;

		private RouteVehicleChoiceController _routeVehicleChoiceController;

		private void Start()
		{
			_transportRouteElements = new List<RouteElementView>();
			_routeVehicleChoiceController = FindObjectOfType<RouteVehicleChoiceController>();
			_userInformationPopup = FindObjectOfType<UserInformationPopup>();
		}

		public InputField RouteNameField {
			get {
				return _routeNameField;
			}

			set {
				_routeNameField = value;
			}
		}

		public List<RouteElementView> TransportRouteElementViews
		{
			get
			{
				List<RouteElementView> routeElementViews = new List<RouteElementView>();
				for (int i = 0; i < _routeElementScrollView.ContentObjects.Count; i++)
				{
					routeElementViews.Add(GetElementView(i));
				}
				return routeElementViews;
			}
		}

		public RouteElementView SelectedRouteElement {
			get {
				return _selectedRouteElement;
			}

			set {
				_selectedRouteElement = value;
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

		private RouteElementView GetElementView(int index)
		{
			return _routeElementScrollView.ContentObjects[index].gameObject.GetComponent<RouteElementView>();
		}

		public void OnTransportStationClick(PathFindingNode pathFindingNode)
		{
			if (!gameObject.activeSelf) return;
			if (_routeVehicleChoiceController.SelectedVehicle == null)
			{
				_userInformationPopup.InformationText = "Vehicle needs to be set first.";
				return;
			}
			GameObject elementView = _routeElementScrollView.AddObject((RectTransform)_routeElementPrefab.transform);
			RouteElementView view = elementView.GetComponent<RouteElementView>();
			view.FromNode = pathFindingNode;
			if (_routeElementScrollView.ContentObjects.Count > 1)
			{
				RouteElementView routeElementViewFirst = GetElementView(0);
				RouteElementView routeElementViewSecondLast = GetElementView(_routeElementScrollView.ContentObjects.Count - 2);
				RouteElementView routeElementViewLast = GetElementView(_routeElementScrollView.ContentObjects.Count - 1);
				routeElementViewSecondLast.ToNode = routeElementViewLast.FromNode;
				routeElementViewLast.ToNode = routeElementViewFirst.FromNode;
			}
		}

		public void LoadTransportRoute(TransportRoute transportRoute)
		{
			foreach (TransportRouteElement transportRouteElement in transportRoute.TransportRouteElements)
			{
				GameObject elementView = _routeElementScrollView.AddObject((RectTransform)_routeElementPrefab.transform);
				RouteElementView view = elementView.GetComponent<RouteElementView>();
				view.TransportRouteElement = transportRouteElement;
			}
		}

		public void RemoveTransportRouteElement(RouteElementView routeElementView)
		{
			_routeElementScrollView.RemoveObject((RectTransform)routeElementView.transform);
			for (int i = 0; i < _routeElementScrollView.ContentObjects.Count; i++)
			{
				RouteElementView view = GetElementView(i);
				if (view.ToNode != routeElementView.FromNode) continue;
				view.ToNode = GetElementView((i + 1) % _routeElementScrollView.ContentObjects.Count).FromNode;
				break;
			}
		}

		public void Reset()
		{
			_routeElementScrollView.ClearObjects();
			RouteNameField.text = "";
		}
	}
}