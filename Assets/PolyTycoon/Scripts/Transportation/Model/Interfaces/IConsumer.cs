using System.Collections.Generic;

public interface IConsumer
{
	 Dictionary<ProductData, ProductStorage> NeededProducts();
}
