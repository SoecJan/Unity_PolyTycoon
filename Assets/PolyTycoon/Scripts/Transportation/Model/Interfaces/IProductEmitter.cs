using System.Collections.Generic;
using JetBrains.Annotations;

public interface IProductEmitter
{
    ProductStorage EmitterStorage([CanBeNull] ProductData productData = null);
    
    List<ProductData> EmittedProductList();
}