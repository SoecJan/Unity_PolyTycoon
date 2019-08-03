using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[Serializable]
public class RouteCreationVehicleManager
{
     #region Attributes

    private TransportRouteCreateController _routeCreateController;
    private TransportVehicle _selectedVehicle;
    public System.Action<TransportVehicle> OnVehicleSelect;
    
    [SerializeField] private Text _vehicleChoiceTitleText;
    [SerializeField] private GameObject _vehicleChoiceVisibleGameObject;
    [SerializeField] private VehicleOptionView _vehicleOptionViewPrefab;
    [SerializeField] private Transform _vehicleChoiceScrollViewTransform;
    [SerializeField] private ToggleGroup _vehicleChoiceToggleGroup;
    [SerializeField] private Text _speedText;
    [SerializeField] private Text _strengthText;
    [SerializeField] private Text _capacityText;
    [SerializeField] private Text _unloadSpeedText;
    [SerializeField] private Text _costText;
    [SerializeField] private Text _dailyCostText;

    public void Initialize()
    {
        FillVehicleView();
    }
    
    public TransportVehicle SelectedVehicle
    {
        get { return _selectedVehicle; }

        set
        {
            _selectedVehicle = value;
            UpdateUi(SelectedVehicle);
        }
    }

    public GameObject VehicleChoiceVisibleGameObject
    {
        get { return _vehicleChoiceVisibleGameObject; }

        set { _vehicleChoiceVisibleGameObject = value; }
    }

    

    #endregion

    #region Standard Methods

    public void Reset()
    {
        VehicleChoiceVisibleGameObject.SetActive(true);

        SelectedVehicle = null;
        _vehicleChoiceTitleText.text = "Vehicle Amount";
        _speedText.text = "-";
        _strengthText.text = "-";
        _capacityText.text = "-";
        _unloadSpeedText.text = "-";
        _costText.text = "-";
        _dailyCostText.text = "-";
    }

    #endregion

    private void OnVehicleSelectClick(Vehicle vehicle)
    {
        Debug.Log("Vehicle selected: " + vehicle.name);
        if (vehicle is TransportVehicle)
        {
            SelectedVehicle = (TransportVehicle) vehicle;
            OnVehicleSelect(SelectedVehicle);
        }
    }

    private void FillVehicleView()
    {
        VehicleManager vehicleManager = Object.FindObjectOfType<VehicleManager>();
        foreach (Vehicle vehicle in vehicleManager.VehicleList)
        {
            VehicleOptionView vehicleOptionObject =
                GameObject.Instantiate(_vehicleOptionViewPrefab, _vehicleChoiceScrollViewTransform);
            vehicleOptionObject.Vehicle = vehicle;
            vehicleOptionObject.SelectToggle.onValueChanged.AddListener(delegate(bool isActive)
            {
                if (isActive)
                {
                    OnVehicleSelectClick(vehicleOptionObject.Vehicle);
                }
            });
            vehicleOptionObject.SelectToggle.group = _vehicleChoiceToggleGroup;
        }
    }

    private void UpdateUi(TransportVehicle vehicle)
    {
        if (!vehicle) return;
        _vehicleChoiceTitleText.text = vehicle.name + " Amount";
        _speedText.text = "-";
        _strengthText.text = "-";
        _capacityText.text = "-";
        _unloadSpeedText.text = "-";
        _costText.text = "-";
        _dailyCostText.text = "-";
    }
}