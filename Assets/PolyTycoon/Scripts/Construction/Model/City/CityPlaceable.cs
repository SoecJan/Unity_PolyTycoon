﻿using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Holds a reference to all buildings contained in this cityPlaceable. 
/// </summary>
public class CityPlaceable : ComplexMapPlaceable, IConsumer, IProducer
{
	#region Attributes

	private Dictionary<ProductData, ProductStorage> _neededProductStorages;
	private Vector2 _centerPosition; // Center Position of this CityPlaceable
	private int _size;

	[SerializeField] private CityMainBuilding _mainBuilding;
	[SerializeField] private ProductData _producedProduct;
	private ProductStorage _productStorage;
	#endregion

	#region Getter & Setter
	Dictionary<ProductData, ProductStorage> IConsumer.NeededProducts()
	{
		return _neededProductStorages;
	}
	
	public ProductStorage ProducedProductStorage()
	{
		return _productStorage;
	}

	public Vector2 CenterPosition {
		get {
			return _centerPosition;
		}

		set {
			_centerPosition = value;
		}
	}

	public int Size {
		get {
			return _size;
		}

		set {
			_size = value;
		}
	}

	public CityMainBuilding MainBuilding {
		get {
			return _mainBuilding;
		}

		set {
			_mainBuilding = value;
		}
	}
	#endregion

	#region Methods

	protected override void Initialize()
	{
		if (ChildMapPlaceables.Count == 0)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				SimpleMapPlaceable simpleMapPlaceable = transform.GetChild(i).gameObject.GetComponent<SimpleMapPlaceable>();
				if (simpleMapPlaceable) ChildMapPlaceables.Add(simpleMapPlaceable);
				if (simpleMapPlaceable is CityMainBuilding && !_mainBuilding) _mainBuilding = (CityMainBuilding)simpleMapPlaceable;
			}
		}
		_neededProductStorages = new Dictionary<ProductData, ProductStorage>();
		_productStorage = new ProductStorage(_producedProduct, 3);
		transform.eulerAngles = new Vector3Int(0, Random.Range(0, 4) * 90, 0);
		foreach (SimpleMapPlaceable simpleMapPlaceable in ChildMapPlaceables)
		{
			if (!(simpleMapPlaceable is CityBuilding)) continue;
			CityBuilding cityBuilding = ((CityBuilding)simpleMapPlaceable);
			cityBuilding.CityPlaceable = this;
			foreach (NeededProduct neededProduct in cityBuilding.ConsumedProducts)
			{
				if (_neededProductStorages.ContainsKey(neededProduct.Product))
				{
					_neededProductStorages[neededProduct.Product].MaxAmount += neededProduct.Amount;
				}
				else
				{
					_neededProductStorages.Add(neededProduct.Product, new ProductStorage(neededProduct.Product, neededProduct.Amount));
				}
			}
		}
	}

	/// <summary>
	/// Moves the CityPlaceable by given value
	/// </summary>
	/// <param name="x">x offset</param>
	/// <param name="y">y offset</param>
	/// <param name="z">z offset</param>
	public void Translate(int x, float y, int z)
	{
		_centerPosition.x += x;
		_centerPosition.y += z;
		transform.Translate(x, y, z);
	}

	/// <summary>
	/// In game height of this object.
	/// </summary>
	/// <returns></returns>
	public float GetHeight()
	{
		return GetComponentInChildren<Transform>().lossyScale.y;
	}

	/// <summary>
	/// Count of people living in this cityPlaceable.
	/// </summary>
	/// <returns></returns>
	public int CurrentInhabitantCount()
	{
		int result = 0;
		foreach (SimpleMapPlaceable mapPlaceable in ChildMapPlaceables)
		{
			if (!(mapPlaceable is CityBuilding)) continue;
			result += ((CityBuilding)mapPlaceable).CurrentResidentCount;
		}
		return result;
	}
	#endregion
}
