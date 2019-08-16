using System.Collections.Generic;
using JetBrains.Annotations;

/// <summary>
/// This interface defines functionality for entities that need to receive products for production or use.
/// It's the complement to <see cref="IProductEmitter"/>.
/// </summary>
public interface IProductReceiver
{
    /// <summary>
    /// This function makes <see cref="ProductStorage"/> accessible that holds the received products.
    /// </summary>
    /// <param name="productData">(optional) specify the ProductStorage that you want to receive</param>
    /// <returns>The specified ProductStorage.</returns>
    ProductStorage ReceiverStorage([CanBeNull] ProductData productData = null);
    /// <summary>
    /// This function makes all received products accessible
    /// </summary>
    /// <returns>A List of all received products of this entity</returns>
    List<ProductData> ReceivedProductList();
}