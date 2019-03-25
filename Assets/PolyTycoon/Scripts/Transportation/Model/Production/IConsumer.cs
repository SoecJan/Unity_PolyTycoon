﻿using System.Collections;
using System.Collections.Generic;
using Assets.PolyTycoon.Resources.Data.ProductData;
using Assets.PolyTycoon.Scripts.Transportation.Model.Product;
using UnityEngine;

public interface IConsumer
{
	 Dictionary<ProductData, ProductStorage> NeededProducts();
}