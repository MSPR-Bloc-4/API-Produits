using Google.Cloud.Firestore;

namespace Product_Api.Model
{
    [FirestoreData]
    public class Product
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public ProductDetails Details { get; set; }

        [FirestoreProperty]
        public int Stock { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }
    }
}