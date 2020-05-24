public interface IProductManager
{
    ProductData GetRandomProduct();
    ProductData GetProduct(string productName);
    ProductStorage GetProductStorage(string productName, int maxAmount);
}