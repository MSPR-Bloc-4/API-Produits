using Google.Api.Gax;
using Google.Cloud.Firestore;
using Product_Api.Model;
using Product_Api.Repository;

namespace Product_Api.Tests
{
    public class ProductRepositoryTests : IDisposable
    {
        private FirestoreDb _firestoreDb;
        private string _testCollectionName = "Product";

        public ProductRepositoryTests()
        {
            // Initialize FirestoreDb for testing
            var projectId = "payetonkawa-84a8c";
            var builder = new FirestoreDbBuilder
            {
                CredentialsPath = "payetonkawa-84a8c-firebase-adminsdk-syfp3-6e4eaa43f5.json",
                ProjectId = projectId,
                DatabaseId = "test",  // Use the 'test' database for testing
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            };
            _firestoreDb = builder.Build();
        }

        [Fact]
        public async Task CreateProduct_Should_Add_Product_To_Firestore()
        {
            // Arrange
            var productRepository = new ProductRepository(_firestoreDb);
            var product = RandomProductGenerator.GenerateRandomProduct();

            // Act
            var productId = await productRepository.CreateProduct(product);

            // Assert
            Assert.NotNull(productId);
            var retrievedProduct = await GetProductFromFirestore(productId);
            product.Id = productId;
            product.CreatedAt = retrievedProduct.CreatedAt;
            AssertProductProperties(product, retrievedProduct);

            // Clean up
            await productRepository.DeleteProduct(productId);
        }

        [Fact]
        public async Task GetProductById_Should_Retrieve_Product_From_Firestore()
        {
            // Arrange
            var productRepository = new ProductRepository(_firestoreDb);
            var product = RandomProductGenerator.GenerateRandomProduct();
            var productId = await productRepository.CreateProduct(product);

            // Act
            var retrievedProduct = await productRepository.GetProductById(productId);

            // Assert
            Assert.NotNull(retrievedProduct);
            product.Id = retrievedProduct.Id;
            product.CreatedAt = retrievedProduct.CreatedAt;
            AssertProductProperties(product, retrievedProduct);

            // Clean up
            await productRepository.DeleteProduct(productId);
        }

        [Fact]
        public async Task GetAllProducts_Should_Retrieve_All_Products_From_Firestore()
        {
            // Arrange
            var productRepository = new ProductRepository(_firestoreDb);
            var product1 = RandomProductGenerator.GenerateRandomProduct();
            var product2 = RandomProductGenerator.GenerateRandomProduct();
            var productId1 = await productRepository.CreateProduct(product1);
            var productId2 = await productRepository.CreateProduct(product2);

            // Act
            var products = await productRepository.GetAllProducts();

            // Assert
            Assert.NotNull(products);
            Assert.Contains(products, p => p.Id == productId1);
            Assert.Contains(products, p => p.Id == productId2);

            // Clean up
            await productRepository.DeleteProduct(productId1);
            await productRepository.DeleteProduct(productId2);
        }

        [Fact]
        public async Task UpdateProduct_Should_Update_Product_In_Firestore()
        {
            // Arrange
            var productRepository = new ProductRepository(_firestoreDb);
            var product = RandomProductGenerator.GenerateRandomProduct();
            var productId = await productRepository.CreateProduct(product);

            // Modify product details
            product.Name = "Updated Product Name";
            product.Details.Description = "Updated Description";

            // Act
            await productRepository.UpdateProduct(productId, product);
            var updatedProduct = await productRepository.GetProductById(productId);

            // Assert
            Assert.Equal(product.Name, updatedProduct.Name);
            Assert.Equal(product.Details.Description, updatedProduct.Details.Description);

            // Clean up
            await productRepository.DeleteProduct(productId);
        }

        [Fact]
        public async Task DeleteProduct_Should_Delete_Product_From_Firestore()
        {
            // Arrange
            var productRepository = new ProductRepository(_firestoreDb);
            var product = RandomProductGenerator.GenerateRandomProduct();
            var productId = await productRepository.CreateProduct(product);

            // Act
            await productRepository.DeleteProduct(productId);
            var deletedProduct = await productRepository.GetProductById(productId);

            // Assert
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task IncrementStock_Should_Increase_Product_Stock_In_Firestore()
        {
            // Arrange
            var productRepository = new ProductRepository(_firestoreDb);
            var product = RandomProductGenerator.GenerateRandomProduct();
            var productId = await productRepository.CreateProduct(product);

            // Act
            await productRepository.IncrementStock(new List<string> { productId });
            var updatedProduct = await productRepository.GetProductById(productId);

            // Assert
            Assert.Equal(product.Stock + 1, updatedProduct.Stock);

            // Clean up
            await productRepository.DeleteProduct(productId);
        }

        [Fact]
        public async Task DecrementStock_Should_Decrease_Product_Stock_In_Firestore()
        {
            // Arrange
            var productRepository = new ProductRepository(_firestoreDb);
            var product = RandomProductGenerator.GenerateRandomProduct();
            product.Stock = 10; // Set initial stock
            var productId = await productRepository.CreateProduct(product);

            // Act
            await productRepository.DecrementStock(new List<string> { productId });
            var updatedProduct = await productRepository.GetProductById(productId);

            // Assert
            Assert.Equal(product.Stock - 1, updatedProduct.Stock);

            // Clean up
            await productRepository.DeleteProduct(productId);
        }

        private async Task<Product> GetProductFromFirestore(string productId)
        {
            var docRef = _firestoreDb.Collection(_testCollectionName).Document(productId);
            var snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<Product>();
            }
            return null;
        }

        private void AssertProductProperties(Product expected, Product actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Stock, actual.Stock);
            Assert.Equal(expected.CreatedAt, actual.CreatedAt);
            Assert.Equal(expected.Details.Price, actual.Details.Price);
            Assert.Equal(expected.Details.Description, actual.Details.Description);
        }

        public void Dispose()
        {
            // Clean up Firestore data after tests
            ClearFirestoreCollection(_testCollectionName);
        }

        private async void ClearFirestoreCollection(string collectionName)
        {
            var collectionRef = _firestoreDb.Collection(collectionName);
            var query = collectionRef;
            var batch = _firestoreDb.StartBatch();

            // Batch delete documents
            var querySnapshot = await query.GetSnapshotAsync();
            foreach (var documentSnapshot in querySnapshot.Documents)
            {
                batch.Delete(documentSnapshot.Reference);
            }

            // Commit the batch
            await batch.CommitAsync();
        }
    }
}

public static class RandomProductGenerator
{
    private static Random random = new Random();

    public static Product GenerateRandomProduct()
    {
        // Generate a random product ID (assuming it's alphanumeric, adjust as necessary)
        string productId = Guid.NewGuid().ToString().Substring(0, 8);

        // Generate a random product name
        string[] productNames = { "Widget", "Gadget", "Thingamajig", "Doohickey" };
        string productName = productNames[random.Next(productNames.Length)];

        // Generate random product details
        ProductDetails details = new ProductDetails
        {
            Price = (float)(random.NextDouble() * 100), // Random price between 0 and 100
            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit." // Sample description
        };

        // Generate random stock quantity (assuming between 0 and 100)
        int stock = random.Next(101);

        // Generate a random creation date within the last year
        DateTime createdAt = DateTime.UtcNow.AddDays(-random.Next(1, 365));

        // Create the Product object
        Product product = new Product
        {
            Id = productId,
            Name = productName,
            Details = details,
            Stock = stock,
            CreatedAt = createdAt
        };

        return product;
    }
}