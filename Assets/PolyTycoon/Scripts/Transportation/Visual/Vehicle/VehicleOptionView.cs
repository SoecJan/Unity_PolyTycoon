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
		get {
			return _vehicle;
		}

		set {
			_vehicle = value;
			_image.sprite = _vehicle.Sprite;
		}
	}

	public Toggle SelectToggle
	{
		get { return _selectToggle; }
		set { _selectToggle = value; }
	}

	#endregion
}

