using System.Collections;
using System.Collections.Generic;
using Assets.PolyTycoon.Resources.Data.ProductData;
using Assets.PolyTycoon.Scripts.Construction.Model.City;
using Assets.PolyTycoon.Scripts.Transportation.Model.Product;
using UnityEngine;

public class CityMainBuilding : CityBuilding, IConsumer {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Dictionary<ProductData, ProductStorage> NeededProducts()
	{
		return ((IConsumer) CityPlaceable).NeededProducts();
	}
}
