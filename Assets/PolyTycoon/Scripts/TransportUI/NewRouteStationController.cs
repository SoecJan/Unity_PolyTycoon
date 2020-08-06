using System;
using System.Collections.Generic;
using UnityEngine;

public class NewRouteStationController : MonoBehaviour
{
    [SerializeField] private RouteElementView _elementPrefab;
    [SerializeField] private RectTransform _parentTransform;
    private IPathFinder _pathFinder;

    public IPathFinder PathFinder
    {
        set => _pathFinder = value;
    }

    private void Start()
    {
        RouteElementView._onDelete += RemoveStation;
    }

    public List<TransportRouteElement> TransportRouteElements
    {
        get { 
            List<TransportRouteElement> output = new List<TransportRouteElement>();
            for (int i = 0; i < _parentTransform.childCount; i++)
            {
                Transform child = _parentTransform.GetChild(i);
                RouteElementView elementView = child.GetComponent<RouteElementView>();
                if (elementView) output.Add(elementView.TransportRouteElement);
            }
            return output; 
        }
    }

    public List<RouteElementView> TransportRouteElementViews
    {
        get
        {
            List<RouteElementView> output = new List<RouteElementView>();
            for (int i = 0; i < _parentTransform.childCount; i++)
            {
                Transform child = _parentTransform.GetChild(i);
                RouteElementView elementView = child.GetComponent<RouteElementView>();
                if (elementView) output.Add(elementView);
            }
            return output;
        }
    }

    public NewRouteVehicleChoiceView VehicleChoiceController { get; set; }

    public void AddStation(List<TransportRouteElement> elementsToAdd)
    {
        RouteElementView _elementView = Instantiate(_elementPrefab, _parentTransform);
        _elementView.transform.SetSiblingIndex(_elementView.transform.GetSiblingIndex()-1);
        switch (elementsToAdd.Count)
        {
            case 1:
                _elementView.TransportRouteElement = elementsToAdd[0];
                break;
            case 2:
                RouteElementView secondlastElementView = 
                    _parentTransform.GetChild(_parentTransform.childCount-3).GetComponent<RouteElementView>();
                secondlastElementView.TransportRouteElement.Path = elementsToAdd[0].Path;
                secondlastElementView.TransportRouteElement.ToNode = elementsToAdd[0].ToNode;
                _elementView.TransportRouteElement = elementsToAdd[1];
                break;
        }
    }
    
    private void RemoveStation(RouteElementView obj)
    {
        List<RouteElementView> elementViews = TransportRouteElementViews;
        int index = elementViews.IndexOf(obj);

        if (elementViews.Count == 2)
        {
            TransportRouteElement routeElement = elementViews[Util.Mod(index + 1, elementViews.Count)].TransportRouteElement;
            routeElement.Path = null;
            routeElement.ToNode = null;
        }
        else
        {
            TransportRouteElement prevRouteElement = elementViews[Util.Mod(index - 1, elementViews.Count)].TransportRouteElement;
            TransportRouteElement nextRouteElement = elementViews[Util.Mod(index + 1, elementViews.Count)].TransportRouteElement;

            prevRouteElement.ToNode = nextRouteElement.FromNode;
            prevRouteElement.Path = null;
            List<TransportRouteElement> routeElements = new List<TransportRouteElement>() { prevRouteElement, nextRouteElement};
            ThreadedDataRequester.RequestData(() => _pathFinder.FindPath(VehicleChoiceController.SelectedVehicle, routeElements));
        }
        Destroy(obj.gameObject);
    }

    public void OnStationClick(PathFindingTarget pathFindingTarget)
    {
        List<RouteElementView> elementViews = TransportRouteElementViews;
        List<TransportRouteElement> elements;
        if (elementViews.Count == 0)
        {
            TransportRouteElement transportRouteElement = new TransportRouteElement() { FromNode = pathFindingTarget };
            elements = new List<TransportRouteElement>() { transportRouteElement };
            AddPath(elements);
        }
        else
        {
            TransportRouteElement transportRouteElement = new TransportRouteElement()
            {
                FromNode = elementViews[elementViews.Count-1].TransportRouteElement.FromNode, ToNode = pathFindingTarget
            };
            TransportRouteElement lastTransportRouteElement = new TransportRouteElement()
            {
                FromNode = pathFindingTarget, ToNode = elementViews[0].TransportRouteElement.FromNode
            };
            elements = new List<TransportRouteElement>() {transportRouteElement, lastTransportRouteElement};
            ThreadedDataRequester.RequestData(() => _pathFinder.FindPath(VehicleChoiceController.SelectedVehicle, elements), AddPath);
        }
    }

    private void AddPath(object result)
    {
        List<TransportRouteElement> transportRouteElements = (List<TransportRouteElement>) result;
        AddStation(transportRouteElements);
    }

    public void Reset()
    {
        foreach (RouteElementView routeElementView in TransportRouteElementViews)
        {
            Destroy(routeElementView.gameObject);
        }
    }
}
