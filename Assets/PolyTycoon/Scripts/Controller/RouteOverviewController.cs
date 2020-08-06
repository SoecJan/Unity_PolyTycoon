
using System.Collections.Generic;
using NUnit.Framework;

public class RouteOverviewController
{
    private RouteOverviewView _view;
    private ITransportRouteManager _manager;
    
    public RouteOverviewController(ITransportRouteManager transportRouteManager, RouteOverviewView routeOverviewView)
    {
        _manager = transportRouteManager;
        _view = routeOverviewView;
        _view._exitButton.onClick.AddListener(delegate
        {
            _view._visibleObject.gameObject.SetActive(false);
        });
        
        _view._showButton.onClick.AddListener(delegate
        {
            if (!_view._visibleObject.gameObject.activeSelf)
            {
                FillView(PathType.None);
            }
            _view._visibleObject.gameObject.SetActive(!_view._visibleObject.gameObject.activeSelf);
        });
        
        _view._sortingAllButton.onClick.AddListener(delegate { FillView(PathType.None); });
        
        _view._sortingRoadButton.onClick.AddListener(delegate { FillView(PathType.Road); });
        
        _view._sortingRailButton.onClick.AddListener(delegate { FillView(PathType.Rail); });
        
        _view._sortingSeaButton.onClick.AddListener(delegate { FillView(PathType.Water); });
        
        _view._sortingAirButton.onClick.AddListener(delegate { FillView(PathType.Air); });
    }
    
    private void FillView(PathType pathType)
    {
        _view._foldableList.Clear();
        List<TransportRoute> routes = _manager.GetRoutes(pathType);
        if (routes == null || routes.Count == 0)
        {
            _view._searchResultInfo.text = "0 " + (pathType == PathType.None ? "" : pathType.ToString() + " ") + "routes found. Create one!";
            return;
        }
        int vehicleCount = 0;
        foreach (TransportRoute transportRoute in routes)
        {
            vehicleCount += transportRoute.TransportVehicles.Count;
            FoldableElement foldableElement = _view._foldableList.Add();
            RouteOverviewElement routeOverviewElement = foldableElement.GetComponent<RouteOverviewElement>();
            routeOverviewElement.TransportRoute = transportRoute;
        }
        _view._searchResultInfo.text = routes.Count + " Routes, " + vehicleCount + " Vehicles";
    }
}
