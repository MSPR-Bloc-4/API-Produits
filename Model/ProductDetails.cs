using Google.Cloud.Firestore;

namespace Product_Api.Model;

[FirestoreData]
public class ProductDetails
{
    [FirestoreProperty]
    public float Price { get; set; }

    [FirestoreProperty]
    public string Description { get; set; }
}