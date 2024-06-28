using Product_Api.Model;

namespace Product_Api.Repository.Interface;

public interface IProductRepository
{
    Task<string> CreateProduct(Product product);
    Task<Product> GetProductById(string productId);
    Task<List<Product>> GetAllProducts();
    Task UpdateProduct(string productId, Product updatedProduct);
    Task DeleteProduct(string productId);
}