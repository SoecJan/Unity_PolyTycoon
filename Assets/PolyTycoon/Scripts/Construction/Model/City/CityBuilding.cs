using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class of any Building that can be registered at a <see cref="CityPlaceable"/> object.
/// </summary>
public class CityBuilding : PathFindingTarget, ICityBuilding
{
	#region Attributes
	[SerializeField] private List<NeededProduct> _consumedProducts;
	[SerializeField] private CityPlaceable _cityPlaceable;
	#endregion

	#region Getter & Setter

	public int CurrentResidentCount { get; set; } = 3;

	public CityPlaceable CityPlaceable()
	{
		return _cityPlaceable;
	}

	public List<NeededProduct> ConsumedProducts {
		get {
			return _consumedProducts;
		}

		set {
			_consumedProducts = value;
		}
	}
	#endregion

	protected override void Initialize()
	{
		_isClickable = true;
		if (!_cityPlaceable && transform.parent) _cityPlaceable = transform.parent.gameObject.GetComponent<CityPlaceable>();
		RotateUsedCoords(transform.eulerAngles.y);
	}
}