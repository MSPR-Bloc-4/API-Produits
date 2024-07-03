using Product_Api.Model;
using Product_Api.Repository.Interface;
using Product_Api.Service.Interface;

namespace Product_Api.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<string> CreateProduct(Product product)
        {
            return await _productRepository.CreateProduct(product);
        }

        public async Task<Product> GetProductById(string productId)
        {
            return await _productRepository.GetProductById(productId);
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _productRepository.GetAllProducts();
        }

        public async Task UpdateProduct(string productId, Product updatedProduct)
        {
            await _productRepository.UpdateProduct(productId, updatedProduct);
        }

        public async Task DeleteProduct(string productId)
        {
            await _productRepository.DeleteProduct(productId);
        }
        
        public async Task IncrementStock(List<string> productIds)
        {
            await _productRepository.IncrementStock(productIds);
        }
        public async Task DecrementStock(List<string> productIds)
        {
            await _productRepository.DecrementStock(productIds);
        }
    }
}