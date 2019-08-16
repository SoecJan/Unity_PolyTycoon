using System.Collections.Generic;
using JetBrains.Annotations;

/// <summary>
/// This interface specifies functionality for <see cref="Factory"/>, <see cref="CityPlaceable"/> and <see cref="Warehouse"/>.
/// It's purpose is to make ProductStorages accessible to display to the user, like <see cref="FactoryView"/>,
/// <see cref="CityView"/> and <see cref="StorageContainerView"/>. Transportation logic heavily relies on this
/// functionality in <see cref="TransportVehicle"/>.
/// </summary>
public interface IProductEmitter
{
    /// <summary>
    /// <see cref="ProductStorage"/> of an emitted product. Products can be specified in the parameter but is not
    /// needed if there is only one emitted product.
    /// </summary>
    /// <param name="productData">May be null. Can be used to specify a product to get the productdata</param>
    /// <returns>
    /// Null: No Product is emitted,
    /// If input null: Single emitted product - Null if there is more than one product emitted
    /// If input value: ProductStorage of the specified product or null if there is none
    /// </returns>
    ProductStorage EmitterStorage([CanBeNull] ProductData productData = null);
    
    /// <summary>
    /// List of products that are emitted by this emitter.
    /// </summary>
    /// <returns>New list that contains products that this emitter emits.</returns>
    List<ProductData> EmittedProductList();
}