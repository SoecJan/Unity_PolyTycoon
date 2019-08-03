using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class VehicleOptionView : MonoBehaviour
{
	#region Attributes
	private Vehicle _vehicle;
	[SerializeField] private Toggle _selectToggle;
	[SerializeField] private Image _image;
	#endregion

	#region Getter & Setter
	public Vehicle Vehicle {
		get => _vehicle;

		set {
			_vehicle = value;
			_image.sprite = _vehicle.Sprite;
		}
	}

	public Toggle SelectToggle => _selectToggle;
	#endregion
}

