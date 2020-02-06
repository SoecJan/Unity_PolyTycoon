using System.Collections.Generic;
using UnityEngine;

public interface IProductManager
{
    ProductData GetRandomProduct();
    ProductData GetProduct(string productName);
    ProductStorage GetProductStorage(string productName, int maxAmount);
}

public class ProductManager : MonoBehaviour, IProductManager
{
    #region Attributes

    [SerializeField] private List<ProductData> products;

    #endregion

    #region Getter & Setter

    public List<ProductData> Products => products;

    public ProductData GetRandomProduct()
    {
        return products[Random.Range(0, products.Count - 1)];
    }

    public ProductData GetProduct(string productName)
    {
        foreach (ProductData product in products)
        {
            if (product.ProductName.Equals(productName))
            {
                return product;
            }
        }

        return null;
    }

    public ProductStorage GetProductStorage(string productName, int maxAmount)
    {
        foreach (ProductData product in products)
        {
            if (product.ProductName.Equals(productName))
            {
                ProductStorage output = new ProductStorage
                {
                    StoredProductData = product,
                    MaxAmount = maxAmount,
                };
                output.SetAmount(0);
                return output;
            }
        }

        return null;
    }

    #endregion
}