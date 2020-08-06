using System;
using System.Collections.Generic;
using NUnit.Framework.Internal.Commands;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouteOverviewElementChild : MonoBehaviour
{
    private TransportVehicle _transportVehicle;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _locateButton;
    [SerializeField] private TMP_Text _locationText;
    [SerializeField] private TMP_Text _loadText;

    public TransportVehicle TransportVehicle
    {
        get => _transportVehicle;
        set => _transportVehicle = value;
    }

    private void Start()
    {
        _deleteButton.onClick.AddListener(delegate
        {
            TransportRoute transportRoute = _transportVehicle.TransportRoute;
            GameHandler gameHandler = FindObjectOfType<GameHandler>();
            ITransportRouteManager transportRouteManager = gameHandler.TransportRouteManager;
            IVehicleManager vehicleManager = FindObjectOfType<GameHandler>().VehicleManager;
            if (transportRoute.TransportVehicles.Count == 1)
            {
                transportRouteManager.RemoveTransportRoute(transportRoute);
            }
            else
            {
                vehicleManager.RemoveVehicle(_transportVehicle);
            }
            Destroy(this.gameObject);
        });
        
        _locateButton.onClick.AddListener(delegate
        {
            Camera.main.GetComponent<CameraBehaviour>().SetTarget(_transportVehicle.transform);
        });
    }

    private void Update()
    {
        if (_transportVehicle == null) return;

        string loadText = "";
        foreach (ProductData productData in _transportVehicle.LoadedProducts)
        {
            loadText += productData.ProductName + " (" + _transportVehicle.TransportStorage(productData).Amount + ") ";
        }
        _loadText.text = loadText;

        List<WayPoint> waypoints =
            _transportVehicle.RouteMover.PathList[_transportVehicle.RouteMover.PathIndex].WayPoints;
        _locationText.text = waypoints[waypoints.Count-1].Node.name;
    }
}
