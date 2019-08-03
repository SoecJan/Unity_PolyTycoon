using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RouteCreationStationManager
{
    #region Attributes
    
    private TransportRouteCreateController _routeCreateController;
    private RouteCreationVehicleManager _routeVehicleChooser;
    private RouteCreationSettingsManager _settingController;

    [SerializeField] private InputField _routeNameField;
    [SerializeField] private GameObject _routeElementVisibleGameObject;
    [SerializeField] private Transform _routeElementScrollView;
    [SerializeField] private TransportRouteElementView _routeElementPrefab;
    [SerializeField] private IUserInformationPopup _routeElementUserInformationPopup;
    
    public string RouteName
    {
        get => _routeNameField.text;
        set => _routeNameField.text = value;
    }

    public List<TransportRouteElementView> TransportRouteElementViews
    {
        get
        {
            if (SelectedRouteElement) _settingController.Save(SelectedRouteElement.RouteElement);
            List<TransportRouteElementView> routeElementViews = new List<TransportRouteElementView>();
            for (int i = 0; i < _routeElementScrollView.childCount; i++)
            {
                routeElementViews.Add(GetElementView(i));
            }

            return routeElementViews;
        }
    }

    public List<TransportRouteElement> TransportRouteElements
    {
        get
        {
            if (SelectedRouteElement) _settingController.Save(SelectedRouteElement.RouteElement);
            List<TransportRouteElement> routeElements = new List<TransportRouteElement>();
            for (int i = 0; i < _routeElementScrollView.childCount; i++)
            {
                routeElements.Add(GetElementView(i).RouteElement);
            }

            return routeElements;
        }
    }

    public TransportRouteElementView SelectedRouteElement { get; set; }

    public GameObject RouteElementVisibleGameObject => _routeElementVisibleGameObject;

    #endregion

    #region Standard Methods

    public void Reset()
    {
        for (int i = 0; i < _routeElementScrollView.childCount; i++)
        {
            TransportRouteElementView element = GetElementView(i);
            GameObject.Destroy(element.gameObject);
        }

        RouteName = "";
        RouteElementVisibleGameObject.SetActive(false);
    }

    #endregion

    public TransportRouteCreateController RouteCreateController
    {
        set
        {
            _routeCreateController = value;
            _routeVehicleChooser = value.VehicleManager;
            _settingController = value.SettingController;
            _routeElementUserInformationPopup = value.UserInformationPopup;
            _routeNameField.onEndEdit.AddListener(delegate { value.IsReady(); });
        }
    }

    private TransportRouteElementView GetElementView(int index)
    {
        return _routeElementScrollView.GetChild(index).gameObject.GetComponent<TransportRouteElementView>();
    }

    public void OnTransportStationClick(PathFindingNode pathFindingNode)
    {
        if (!RouteElementVisibleGameObject.activeSelf) return;
        if (_routeVehicleChooser.SelectedVehicle == null)
        {
            _routeElementUserInformationPopup.InformationText = "Vehicle needs to be set first.";
            return;
        }

        if (_routeElementScrollView.childCount > 0 &&
            GetElementView(_routeElementScrollView.childCount - 1).FromNode == pathFindingNode)
        {
            _routeElementUserInformationPopup.InformationText = "Route needs to have unique stations.";
            return;
        }

        AddNode(pathFindingNode);

        _routeCreateController.IsReady();
    }

    private void AddNode(PathFindingNode pathFindingNode)
    {
        TransportRouteElementView elementView = GameObject.Instantiate(_routeElementPrefab, _routeElementScrollView);
        elementView.SelectButton.onClick.AddListener(delegate
        {
            if (SelectedRouteElement)
            {
                _settingController.Save(SelectedRouteElement.RouteElement);
            }

            SelectedRouteElement = elementView;
            _settingController.LoadRouteElementSettings(elementView.RouteElement);
            Debug.Log("TransportElement selected; Setting Amount: " + elementView.RouteElement.RouteSettings.Count);
        });
        elementView.FromNode = pathFindingNode;
        if (_routeElementScrollView.childCount <= 1) return;
        TransportRouteElementView routeElementViewFirst = GetElementView(0);
        TransportRouteElementView routeElementViewSecondLast = GetElementView(_routeElementScrollView.childCount - 2);
        TransportRouteElementView routeElementViewLast = GetElementView(_routeElementScrollView.childCount - 1);
        routeElementViewSecondLast.ToNode = routeElementViewLast.FromNode;
        routeElementViewLast.ToNode = routeElementViewFirst.FromNode;
        if (_routeElementScrollView.childCount == 1)
        {
            SelectedRouteElement = GetElementView(0);
            _settingController.LoadRouteElementSettings(SelectedRouteElement.RouteElement);
        }

        _settingController.RouteSettingVisibleGameObject.SetActive(true);
    }

    public void LoadTransportRoute(TransportRoute transportRoute)
    {
        foreach (TransportRouteElement transportRouteElement in transportRoute.TransportRouteElements)
        {
            AddNode(transportRouteElement.FromNode);
        }
    }

    public void RemoveTransportRouteElement(TransportRouteElementView routeElementView)
    {
        for (int i = 0; i < _routeElementScrollView.childCount; i++)
        {
            TransportRouteElementView element = GetElementView(i);
            if (element.Equals(routeElementView))
            {
                GameObject.Destroy(element.gameObject);
                break;
            }
        }

        for (int i = 0; i < _routeElementScrollView.childCount; i++)
        {
            TransportRouteElementView view = GetElementView(i);
            if (view.ToNode != routeElementView.FromNode) continue;
            view.ToNode = GetElementView((i + 1) % _routeElementScrollView.childCount).FromNode;
            break;
        }
    }
}