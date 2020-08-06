using UnityEngine;
using UnityEngine.UI;

public class NewRouteVehicleChoiceView : MonoBehaviour
{
    [SerializeField] private Image _selectedVehicleImage;
    [SerializeField] private Button _dropdownButton;
    [SerializeField] private Transform _dropdownTransform;
    [SerializeField] private Button _roadVehicleChoiceButton;
    [SerializeField] private Button _railVehicleChoiceButton;
    [SerializeField] private Button _seaVehicleChoiceButton;
    [SerializeField] private Button _airVehicleChoiceButton;
    
    private TransportVehicleData _selectedVehicle;
    private TransportVehicleData _defaultVehicle;

    public TransportVehicleData[] Vehicles
    {
        set
        {
            foreach (TransportVehicleData vehicleData in value)
            {
                switch (vehicleData.PathType)
                {
                    case PathType.Road:
                        _selectedVehicle = vehicleData;
                        _defaultVehicle = vehicleData;
                        _roadVehicleChoiceButton.onClick.AddListener(delegate
                        {
                            SelectedVehicle = vehicleData; 
                            _dropdownTransform.gameObject.SetActive(false);
                        });
                        break;
                    case PathType.Rail:
                        _railVehicleChoiceButton.onClick.AddListener(delegate
                        {
                            SelectedVehicle = vehicleData; 
                            _dropdownTransform.gameObject.SetActive(false);
                        });
                        break;
                    case PathType.Water:
                        _seaVehicleChoiceButton.onClick.AddListener(delegate
                        {
                            SelectedVehicle = vehicleData; 
                            _dropdownTransform.gameObject.SetActive(false);
                        });
                        break;
                    case PathType.Air:
                        _airVehicleChoiceButton.onClick.AddListener(delegate
                        {
                            SelectedVehicle = vehicleData; 
                            _dropdownTransform.gameObject.SetActive(false);
                        });
                        break;
                }
            }
        }
    }

    public TransportVehicleData SelectedVehicle
    {
        get => _selectedVehicle;
        set
        {
            _selectedVehicle = value;
            _selectedVehicleImage.sprite = _selectedVehicle.Sprite;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _dropdownButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        _dropdownTransform.gameObject.SetActive(!_dropdownTransform.gameObject.activeSelf);
    }

    public void Reset()
    {
        SelectedVehicle = _defaultVehicle;
    }
}
