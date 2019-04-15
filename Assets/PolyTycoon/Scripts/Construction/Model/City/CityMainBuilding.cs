using System.Collections.Generic;

public class CityMainBuilding : CityBuilding, IConsumer {

	public Dictionary<ProductData, ProductStorage> NeededProducts()
	{
		return ((IConsumer) CityPlaceable).NeededProducts();
	}
}
