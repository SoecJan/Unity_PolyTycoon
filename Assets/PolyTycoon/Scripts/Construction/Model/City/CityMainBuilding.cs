using System.Collections.Generic;
using UnityEngine;

public class CityMainBuilding : PathFindingTarget, IConsumer, ICityBuilding
{
	[SerializeField] private CityPlaceable _cityPlaceable;

	protected override void Initialize()
	{
		base.Initialize();
		RotateUsedCoords(transform.eulerAngles.y);
		if (!_cityPlaceable && transform.parent) _cityPlaceable = transform.parent.gameObject.GetComponent<CityPlaceable>();
	}

	public Dictionary<ProductData, ProductStorage> NeededProducts()
	{
		return ((IConsumer) _cityPlaceable).NeededProducts();
	}

	public CityPlaceable CityPlaceable()
	{
		return _cityPlaceable;
	}
}
