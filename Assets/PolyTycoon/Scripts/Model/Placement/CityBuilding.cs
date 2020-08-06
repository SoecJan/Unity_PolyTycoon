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
}

/// <summary>
/// Class of any Building that can be registered at a <see cref="CityPlaceable"/> object.
/// </summary>
public class CityBuilding : SimpleMapPlaceable, ICityBuilding
{
	#region Getter & Setter

	public CityPlaceable CityPlaceable { get; set; }
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
	
	void Awake()
	{
		_isClickable = true;
		if (!CityPlaceable && transform.parent) CityPlaceable = transform.parent.gameObject.GetComponent<CityPlaceable>();
		RotateUsedCoords(transform.eulerAngles.y);
	}
	#endregion
	
}