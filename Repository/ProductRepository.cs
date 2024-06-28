using Google.Cloud.Firestore;
using Product_Api.Model;
using Product_Api.Repository.Interface;

namespace Product_Api.Repository;

public class ProductRepository : IProductRepository
{
    private readonly FirestoreDb _firestoreDb;

    public ProductRepository(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public async Task<string> CreateProduct(Product product)
    {
        CollectionReference productsRef = _firestoreDb.Collection("Products");
        DocumentReference docRef = await productsRef.AddAsync(product);

        return docRef.Id;
    }

    public async Task<Product> GetProductById(string productId)
    {
        DocumentReference docRef = _firestoreDb.Collection("Products").Document(productId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<Product>();
        }

        return null;
    }

    public async Task<List<Product>> GetAllProducts()
    {
        QuerySnapshot snapshot = await _firestoreDb.Collection("Products").GetSnapshotAsync();
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
        DocumentReference docRef = _firestoreDb.Collection("Products").Document(productId);
        await docRef.SetAsync(updatedProduct, SetOptions.MergeAll);
    }

    public async Task DeleteProduct(string productId)
    {
        DocumentReference docRef = _firestoreDb.Collection("Products").Document(productId);
        await docRef.DeleteAsync();
    }
}