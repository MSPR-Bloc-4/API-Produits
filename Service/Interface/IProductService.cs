using Product_Api.Model;

namespace Product_Api.Service.Interface;

public interface IProductService
{
    Task<string> CreateProduct(Product product);
    Task<Product> GetProductById(string productId);
    Task<List<Product>> GetAllProducts();
    Task UpdateProduct(string productId, Product updatedProduct);
    Task DeleteProduct(string productId);
    Task IncrementStock(List<string> productIds);
    Task DecrementStock(List<string> productIds);
}