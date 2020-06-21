using System;
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
	CityPlaceable CityPlaceable { get; set; }
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
	#endregion

	#region Getter & Setter

	public CityPlaceable CityPlaceable { get; set; }
	public int CurrentResidentCount { get; set; } = 3;

	public List<NeededProduct> ConsumedProducts {
		get => _consumedProducts;

		set => _consumedProducts = value;
	}
	#endregion

	#region Methods
	
	protected override void OnMouseEnter()
	{
		if (!CityPlaceable) return;
		Outline outline = CityPlaceable.gameObject.AddComponent<Outline>();
		if (!outline) outline = CityPlaceable.gameObject.GetComponent<Outline>();
		outline.OutlineMode = Outline.Mode.OutlineVisible;
		outline.OutlineColor = Color.yellow;
		outline.OutlineWidth = 5f;
		outline.enabled = true;
	}

	protected override void OnMouseExit()
	{
		if (CityPlaceable) Destroy(CityPlaceable.gameObject.GetComponent<Outline>());
	}
	
	protected override void Initialize()
	{
		_isClickable = true;
		if (!CityPlaceable && transform.parent) CityPlaceable = transform.parent.gameObject.GetComponent<CityPlaceable>();
		RotateUsedCoords(transform.eulerAngles.y);
	}

	
	#endregion
	
}