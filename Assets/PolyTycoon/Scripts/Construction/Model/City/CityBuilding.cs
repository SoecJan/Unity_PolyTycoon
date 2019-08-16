using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This interface describes the functionality of a building inside a given city.
/// </summary>
public interface ICityBuilding
{
	/// <summary>
	/// A reference to the city instance this entity belongs to.
	/// </summary>
	/// <returns></returns>
	CityPlaceable CityPlaceable();
	/// <summary>
	/// The amount of people living inside the building.
	/// </summary>
	int CurrentResidentCount { get; set; }
}

/// <summary>
/// Class of any Building that can be registered at a <see cref="CityPlaceable"/> object.
/// </summary>
public class CityBuilding : SimpleMapPlaceable, ICityBuilding
{
	#region Attributes
	[SerializeField] private List<NeededProduct> _consumedProducts;
	[SerializeField] private CityPlaceable _cityPlaceable;
	#endregion

	#region Getter & Setter
	public int CurrentResidentCount { get; set; } = 3;

	public List<NeededProduct> ConsumedProducts {
		get => _consumedProducts;

		set => _consumedProducts = value;
	}
	#endregion

	#region Methods
	protected override void Initialize()
	{
		_isClickable = true;
		if (!_cityPlaceable && transform.parent) _cityPlaceable = transform.parent.gameObject.GetComponent<CityPlaceable>();
		RotateUsedCoords(transform.eulerAngles.y);
	}
	
	public CityPlaceable CityPlaceable()
	{
		return _cityPlaceable;
	}
	#endregion
	
}