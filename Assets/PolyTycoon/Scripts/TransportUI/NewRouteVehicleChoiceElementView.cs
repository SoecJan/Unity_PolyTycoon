using System;
using UnityEngine;
using UnityEngine.UI;

public class NewRouteVehicleChoiceElementView : MonoBehaviour
{
    public Action<TransportVehicleData> _OnSelection;
    private TransportVehicleData _transportVehicle;
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;

    public TransportVehicleData TransportVehicle
    {
        get => _transportVehicle;
        set
        {
            _transportVehicle = value;
            _image.sprite = value.Sprite;
        }
    }

    private void Start()
    {
        _button.onClick.AddListener(delegate { _OnSelection?.Invoke(_transportVehicle); });
    }
}
