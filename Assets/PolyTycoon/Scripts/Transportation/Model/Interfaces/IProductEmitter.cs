using System.Collections.Generic;
using JetBrains.Annotations;

public interface IProductEmitter
{
    ProductStorage EmitterStorage([CanBeNull] ProductData productData = null);

    bool IsEmitting(ProductData productData);

    List<ProductData> EmittedProductList();
}