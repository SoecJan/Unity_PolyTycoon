using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class RouteOverviewElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _routeNameText;
    [SerializeField] private Image _vehicleImage;
    [SerializeField] private Button _addVehicleButton;
    [SerializeField] private Button _editRouteButton;
    [SerializeField] private Button _deleteRouteButton;
    [SerializeField] private RectTransform _childParentTransform;
    [SerializeField] private RouteOverviewElementChild _childPrefab;
 
    private static ITransportRouteManager _transportRouteManager;
    private TransportRoute _transportRoute;
    private List<RouteOverviewElementChild> _children;

    public TransportRoute TransportRoute
    {
        get => _transportRoute;
        set
        {
            _transportRoute = value;
            _vehicleImage.sprite = value.TransportVehicles[0].Sprite;
            _routeNameText.text = value.RouteName;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _children = new List<RouteOverviewElementChild>();
        if (_transportRouteManager == null)
        {
            GameHandler gameHandler = FindObjectOfType<GameHandler>();
            _transportRouteManager = gameHandler.TransportRouteManager;
        }
        
        _deleteRouteButton.onClick.AddListener(delegate
        {
            _transportRouteManager.RemoveTransportRoute(_transportRoute);
        });
        
        _editRouteButton.onClick.AddListener(delegate
        {
            _transportRouteManager.EditTransportRoute(_transportRoute);
        });
        
        _addVehicleButton.onClick.AddListener(delegate
        {
            _transportRouteManager.DuplicateTransportRoute(_transportRoute);
        });
        
        FoldableElement foldable = GetComponent<FoldableElement>();
        foldable.OnFoldAction += delegate(FoldableElement element, bool isFolded)
        {
            if (isFolded)
            {
                FillView();
            }
            else
            {
                ClearView();
            }
        };
    }

    private void ClearView()
    {
        for (int i = _children.Count-1; i >= 0; i--)
        {
            Destroy(_children[i].gameObject);
        }
        _children.Clear();
    }

    private void FillView()
    {
        foreach (TransportVehicle transportVehicle in _transportRoute.TransportVehicles)
        {
            RouteOverviewElementChild childObject = Instantiate(_childPrefab, _childParentTransform);
            _children.Add(childObject);
            childObject.TransportVehicle = transportVehicle;
        }
    }
}
