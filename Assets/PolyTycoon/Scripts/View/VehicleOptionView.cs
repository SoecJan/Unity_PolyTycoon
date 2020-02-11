using System;
using UnityEngine;
using UnityEngine.UI;

public class VehicleOptionView : MonoBehaviour
{
	#region Attributes
	private TransportVehicleData _transportVehicleData;
	[SerializeField] private Toggle _selectToggle;
	[SerializeField] private Image _image;
	#endregion

	#region Getter & Setter
	public TransportVehicleData TransportVehicle {
		get => _transportVehicleData;

		set {
			_transportVehicleData = value;
			_image.sprite = value.Sprite;
		}
	}

	public Toggle SelectToggle => _selectToggle;
	#endregion
}

