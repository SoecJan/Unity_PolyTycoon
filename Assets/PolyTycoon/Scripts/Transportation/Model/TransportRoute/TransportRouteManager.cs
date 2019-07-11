﻿using UnityEngine;

public class TransportRouteManager : MonoBehaviour
{
	#region Attributes
	[Header("Controller")]
	[SerializeField] private TransportRouteCreateController _routeCreateController;
	[SerializeField] private TransportRouteOverview _transportRouteOverview;
	[SerializeField] private VehicleManager _vehicleManager;

	private PathFinder _pathFinder;
	private UserInformationPopup _userPopup;
	#endregion

	#region Getter & Setter
	public TransportRouteCreateController RouteCreateController {
		get {
			return _routeCreateController;
		}

		set {
			_routeCreateController = value;
		}
	}
	#endregion

	#region Methods
	void Start()
	{
		_vehicleManager = FindObjectOfType<VehicleManager>();
		_userPopup = FindObjectOfType<UserInformationPopup>();
		_pathFinder = FindObjectOfType<PathFinder>();
	}

	private void Reset()
	{
		RouteCreateController.Reset();
	}

	public void RemoveTransportRoute(TransportRoute transportRoute)
	{
		Destroy(transportRoute.Vehicle.gameObject);
		_transportRouteOverview.Remove(transportRoute);
	}

	public void OnTransportRouteCreate()
	{
		if (!RouteCreateController) RouteCreateController = FindObjectOfType<TransportRouteCreateController>();

		TransportRoute transportRoute = new TransportRoute
		{
			Vehicle = RouteCreateController.VehicleChooser.SelectedVehicle,
			RouteName = RouteCreateController.RouteElementController.RouteNameField.text
		};
		foreach (TransportRouteElementView view in RouteCreateController.RouteElementController.TransportRouteElementViews)
		{
			transportRoute.TransportRouteElements.Add(view.RouteElement);
		}
		PrintNodes(transportRoute);
		ThreadedDataRequester.RequestData(() => _pathFinder.FindPath(transportRoute), FoundPath);
	}

	private void FoundPath(object result)
	{
		TransportRoute transportRoute = (TransportRoute) result;
		OnTransportRoutePathFound(transportRoute);
	}
	
	private void PrintNodes(TransportRoute transportRoute)
	{
		foreach (TransportRouteElement element in transportRoute.TransportRouteElements)
		{
			Debug.Log("From: " + element.FromNode + "; To: " + element.ToNode);
		}
	}

	public void OnTransportRouteChange(TransportRoute transportRoute)
	{
		PrintNodes(transportRoute);
//		_networkPathFinder.FindPath(transportRoute);
		Debug.LogError("TransportRouteChanges need to be reimplemented"); // TODO
	}

	private void OnTransportRouteChangePathFound(TransportRoute transportRoute)
	{
		Debug.Log("Route updated");
		transportRoute.Vehicle.TransportRoute = transportRoute;
		RouteCreateController.SetVisible(false);
		Reset();
	}

	private void OnTransportRoutePathFound(TransportRoute transportRoute)
	{
		if (transportRoute.Vehicle == null)
		{
			_userPopup.InformationText = "Choose a vehicle";
			return;
		}

		if (transportRoute.TransportRouteElements == null || transportRoute.TransportRouteElements.Count <= 1)
		{
			_userPopup.InformationText = "Not enough stations";
			return;
		}
		if ("".Equals(transportRoute.RouteName))
		{
			_userPopup.InformationText = "Route needs to have a name";
			return;
		}
		foreach (TransportRouteElement element in transportRoute.TransportRouteElements)
		{
			if (element.Path != null) continue;
			_userPopup.InformationText = "Stations are not connected";
			return;
		}
		// Add TransportRoute to Overview
		_transportRouteOverview.Add(transportRoute);
		// Instantiate and configure Vehicle
		GameObject instancedVehicle = _vehicleManager.AddVehicle(transportRoute.Vehicle,
			transportRoute.TransportRouteElements[0].Path.WayPoints[0].TraversalVectors[0]);
		TransportVehicle transportVehicle = instancedVehicle.GetComponent<TransportVehicle>();
		transportRoute.Vehicle = transportVehicle;
		transportVehicle.TransportRoute = transportRoute;
		// Reset Route UI
		RouteCreateController.SetVisible(false);
		Reset();
	}
	#endregion
}