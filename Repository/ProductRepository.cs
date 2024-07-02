using Google.Cloud.Firestore;
using Product_Api.Model;
using Product_Api.Repository.Interface;

namespace Product_Api.Repository;

public class ProductRepository : IProductRepository
{
    private readonly FirestoreDb _firestoreDb;
    private readonly CollectionReference _collectionReference;

    public ProductRepository(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
        _collectionReference = _firestoreDb.Collection("Product");
    }

    public async Task<string> CreateProduct(Product product)
    {
        DocumentReference docRef = await _collectionReference.AddAsync(product);

        return docRef.Id;
    }

    public async Task<Product> GetProductById(string productId)
    {
        DocumentReference docRef = _collectionReference.Document(productId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<Product>();
        }

        return null;
    }

    public async Task<List<Product>> GetAllProducts()
    {
        QuerySnapshot snapshot = await _collectionReference.GetSnapshotAsync();
        List<Product> products = new List<Product>();

        foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
        {
            Product product = documentSnapshot.ConvertTo<Product>();
            products.Add(product);
        }

        return products;
    }

    public async Task UpdateProduct(string productId, Product updatedProduct)
    {
        DocumentReference docRef = _collectionReference.Document(productId);
        await docRef.SetAsync(updatedProduct, SetOptions.MergeAll);
    }

    public async Task DeleteProduct(string productId)
    {
        DocumentReference docRef = _collectionReference.Document(productId);
        await docRef.DeleteAsync();
    }
    
    public async Task IncrementStock(List<string> productIds)
    {
        foreach (var productId in productIds)
        {
            var product = await GetProductById(productId);
            if (product != null)
            {
                product.Stock += 1;
                await UpdateProduct(productId, product);
            }
        }
    }
    
    public async Task DecrementStock(List<string> productIds)
    {
        foreach (var productId in productIds)
        {
            var product = await GetProductById(productId);
            if (product != null)
            {
                product.Stock -= 1;
                await UpdateProduct(productId, product);
            }
        }
    }
}