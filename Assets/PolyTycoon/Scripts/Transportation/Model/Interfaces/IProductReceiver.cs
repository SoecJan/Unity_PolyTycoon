using System.Collections.Generic;
using JetBrains.Annotations;

public interface IProductReceiver
{
    ProductStorage ReceiverStorage([CanBeNull] ProductData productData = null);
    List<ProductData> ReceivedProductList();
}