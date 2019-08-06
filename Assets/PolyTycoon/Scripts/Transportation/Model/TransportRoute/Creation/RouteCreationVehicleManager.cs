using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[Serializable]
public class RouteCreationVehicleManager
{
    #region Attributes

    private TransportRouteCreateController _routeCreateController;
    private TransportVehicleData _selectedTransportVehicleData;
    public System.Action<TransportVehicleData> OnVehicleSelect;

    [SerializeField] private TextMeshProUGUI _vehicleChoiceTitleText;
    [SerializeField] private GameObject _vehicleChoiceVisibleGameObject;
    [SerializeField] private VehicleOptionView _vehicleOptionViewPrefab;
    [SerializeField] private Transform _vehicleChoiceScrollViewTransform;
    [SerializeField] private ToggleGroup _vehicleChoiceToggleGroup;
    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private TextMeshProUGUI _strengthText;
    [SerializeField] private TextMeshProUGUI _capacityText;
    [SerializeField] private TextMeshProUGUI _unloadSpeedText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _dailyCostText;

    public void Initialize()
    {
        FillVehicleView();
    }

    public TransportVehicleData SelectedTransportVehicleData
    {
        get { return _selectedTransportVehicleData; }

        set
        {
            _selectedTransportVehicleData = value;
            UpdateUi(SelectedTransportVehicleData);
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

        SelectedTransportVehicleData = null;
        _vehicleChoiceTitleText.text = "Vehicle Choice";
        _speedText.text = "-";
        _strengthText.text = "-";
        _capacityText.text = "-";
        _unloadSpeedText.text = "-";
        _costText.text = "-";
        _dailyCostText.text = "-";
    }

    #endregion

    private void OnVehicleSelectClick(TransportVehicleData transportVehicleData)
    {
        Debug.Log("Vehicle selected: " + transportVehicleData.VehicleName);
        SelectedTransportVehicleData = transportVehicleData;
        OnVehicleSelect(SelectedTransportVehicleData);
    }

    private void FillVehicleView()
    {
        VehicleManager vehicleManager = Object.FindObjectOfType<VehicleManager>();
        foreach (TransportVehicleData transportVehicleData in vehicleManager.VehicleList)
        {
            VehicleOptionView vehicleOptionObject =
                GameObject.Instantiate(_vehicleOptionViewPrefab, _vehicleChoiceScrollViewTransform);
            vehicleOptionObject.TransportVehicle = transportVehicleData;
            vehicleOptionObject.SelectToggle.onValueChanged.AddListener(delegate(bool isActive)
            {
                if (isActive)
                {
                    OnVehicleSelectClick(vehicleOptionObject.TransportVehicle);
                }
            });
            vehicleOptionObject.SelectToggle.group = _vehicleChoiceToggleGroup;
        }
    }

    private void UpdateUi(TransportVehicleData transportVehicleData)
    {
        if (!transportVehicleData) return;
        _vehicleChoiceTitleText.text = transportVehicleData.VehicleName;
        _speedText.text = transportVehicleData.MaxSpeed.ToString();
        _strengthText.text = "-";
        _capacityText.text = transportVehicleData.MaxCapacity.ToString();
        _unloadSpeedText.text = "-";
        _costText.text = "-";
        _dailyCostText.text = "-";
    }
}