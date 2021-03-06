﻿using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class holds necessary information on a given ProductData
/// </summary>
[CreateAssetMenu(fileName = "ProductData", menuName = "PolyTycoon/ProductData", order = 1)]
public class ProductData : ScriptableObject
{
	#region Attributes
	[Header("General")]
	[Tooltip("The product sprite displayed to the player")]
	[SerializeField] private Sprite _productSprite; // The ProductData image
	[Tooltip("The product name displayed to the player")]
	[SerializeField] private string _productName;
	[Tooltip("The product description displayed to the player")]
	[SerializeField] private string _description;
	[Header("Production")]
	[Tooltip("The product time needed to produce one product")]
	[SerializeField] private float _productionTime;
	[Tooltip("The product needed for production")]
	[SerializeField] private NeededProduct[] _neededProduct;
	[Header("Pricing")] 
	[Tooltip("The products base price on which the random factor is added")] 
	[SerializeField] private int _basePrice;
	[Tooltip("A random Factor that is applied to a base price")] 
	[SerializeField] private float _randomPriceFactor;
	#endregion

	#region Getter & Setter
	public Sprite ProductSprite {
		get {
			return _productSprite;
		}
	}

	public string ProductName {
		get {
			return _productName;
		}
	}

	public string Description {
		get {
			return _description;
		}
	}

	public float ProductionTime {
		get {
			return _productionTime;
		}
	}

	public NeededProduct[] NeededProduct {
		get {
			return _neededProduct;
		}
	}

	public int BasePrice
	{
		get => _basePrice;
		set => _basePrice = value;
	}

	public float RandomPriceFactor
	{
		get => _randomPriceFactor;
		set => _randomPriceFactor = value;
	}

	#endregion

	public override string ToString()
	{
		return ProductName + ", Time: " + ProductionTime + ": " + Description;
	}

	public override bool Equals(object obj)
	{
		var data = obj as ProductData;
		return data != null &&
			   base.Equals(obj) && _productName.Equals(data._productName);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}