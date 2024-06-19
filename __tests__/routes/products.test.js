const request = require('supertest');
const { app } = require('../../server');
const Product = require('../../models/Product');
const { connectDatabase, closeDatabase } = require('../../server');

// Mock de mongoose pour simuler les méthodes de modèle
jest.mock('../../models/Product');

// Avant de lancer les tests, connectez-vous à la base de données
beforeAll(async () => {
    // Mock connectDatabase pour supprimer console.log
    jest.spyOn(require('../../server'), 'connectDatabase').mockImplementation(async () => {
        jest.spyOn(console, 'log').mockImplementation(() => {}); // Suppression du console.log dans connectDatabase
        await connectDatabase(); // Appel de la fonction d'origine connectDatabase
    });
});

// Après avoir terminé les tests, fermez la connexion à la base de données
afterAll(async () => {
    await closeDatabase();
});

// Exemple de données de produit pour les tests
const testProductData = {
    name: 'Test Product',
    details: {
        price: '19.99',
        description: 'Test description',
    },
    stock: 10,
};

// Tests pour les routes CRUD de products
describe('Product API', () => {
    // Mock des méthodes de modèle Product
    beforeEach(() => {
        jest.clearAllMocks(); // Réinitialise tous les mocks avant chaque test
    });

    // Test POST /products
    describe('POST /products', () => {
        it('should create a new product', async () => {
            Product.create.mockResolvedValue(testProductData); // Mock pour simuler la création d'un produit
            const response = await request(app)
                .post('/products')
                .send(testProductData);
            expect(response.status).toBe(201);
            expect(response.body.message).toBe('Product created successfully');
            expect(Product.create).toHaveBeenCalledWith(testProductData);
        });

        it('should return 500 when creation fails', async () => {
            Product.create.mockRejectedValue(new Error('Failed to create product')); // Mock pour simuler un échec de création
            const response = await request(app)
                .post('/products')
                .send(testProductData);
            expect(response.status).toBe(500);
            expect(response.body.message).toBe('Failed to create product');
        });        
    });

    // Test GET /products
    describe('GET /products', () => {
        it('should retrieve all products', async () => {
            const mockProducts = [testProductData];
            Product.find.mockResolvedValue(mockProducts); // Mock pour simuler la récupération de tous les produits
            const response = await request(app).get('/products');
            expect(response.status).toBe(200);
            expect(response.body).toEqual(mockProducts);
            expect(Product.find).toHaveBeenCalled();
        });

        it('should return 500 when retrieval fails', async () => {
            Product.find.mockRejectedValue(new Error('Failed to retrieve products')); // Mock pour simuler un échec de récupération
            const response = await request(app).get('/products');
            expect(response.status).toBe(500);
            expect(response.body.message).toBe('Failed to retrieve products');
        });
    });

    // Test GET /products/:productId
    describe('GET /products/:productId', () => {
        it('should retrieve a single product by id', async () => {
            const mockProduct = testProductData;
            Product.findById.mockResolvedValue(mockProduct); // Mock pour simuler la récupération d'un produit par ID
            const response = await request(app).get(`/products/12345`);
            expect(response.status).toBe(200);
            expect(response.body).toEqual(mockProduct);
            expect(Product.findById).toHaveBeenCalledWith('12345');
        });

        it('should return 404 when product is not found by id', async () => {
            Product.findById.mockResolvedValue(null); // Mock pour simuler qu'aucun produit n'est trouvé
            const response = await request(app).get(`/products/12345`);
            expect(response.status).toBe(404);
            expect(response.body.message).toBe('Product not found');
        });

        it('should return 500 when retrieval by id fails', async () => {
            Product.findById.mockRejectedValue(new Error('Failed to retrieve product by id')); // Mock pour simuler un échec de récupération par ID
            const response = await request(app).get(`/products/12345`);
            expect(response.status).toBe(500);
            expect(response.body.message).toBe('Failed to retrieve product');
        });        
    });

    // Test PUT /products/:productId
    describe('PUT /products/:productId', () => {
        it('should update a product', async () => {
            const updatedProductData = { ...testProductData, stock: 20 };
            Product.findByIdAndUpdate.mockResolvedValue(updatedProductData); // Mock pour simuler la mise à jour d'un produit
            const response = await request(app)
                .put(`/products/12345`)
                .send(updatedProductData);
            expect(response.status).toBe(200);
            expect(response.body.message).toBe('Product updated successfully');
            expect(Product.findByIdAndUpdate).toHaveBeenCalledWith(
                '12345',
                updatedProductData,
                { new: true }
            );
        });

        it('should return 404 when product to update is not found', async () => {
            Product.findByIdAndUpdate.mockResolvedValue(null); // Mock pour simuler qu'aucun produit n'est trouvé pour la mise à jour
            const response = await request(app)
                .put(`/products/12345`)
                .send(testProductData);
            expect(response.status).toBe(404);
            expect(response.body.message).toBe('Product not found');
        });

        it('should return 500 when update fails', async () => {
            Product.findByIdAndUpdate.mockRejectedValue(new Error('Failed to update product')); // Mock pour simuler un échec de mise à jour
            const response = await request(app)
                .put(`/products/12345`)
                .send(testProductData);
            expect(response.status).toBe(500);
            expect(response.body.message).toBe('Failed to update product');
        });
    });

    // Test DELETE /products/:productId
    describe('DELETE /products/:productId', () => {
        it('should delete a product', async () => {
            Product.findByIdAndDelete.mockResolvedValue({}); // Mock pour simuler la suppression d'un produit
            const response = await request(app).delete(`/products/12345`);
            expect(response.status).toBe(200);
            expect(response.body.message).toBe('Product deleted successfully');
            expect(Product.findByIdAndDelete).toHaveBeenCalledWith('12345');
        });

        it('should return 404 when product to delete is not found', async () => {
            Product.findByIdAndDelete.mockResolvedValue(null); // Mock pour simuler qu'aucun produit n'est trouvé pour la suppression
            const response = await request(app).delete(`/products/12345`);
            expect(response.status).toBe(404);
            expect(response.body.message).toBe('Product not found');
        });

        it('should return 500 when deletion fails', async () => {
            Product.findByIdAndDelete.mockRejectedValue(new Error('Failed to delete product')); // Mock pour simuler un échec de suppression
            const response = await request(app).delete(`/products/12345`);
            expect(response.status).toBe(500);
            expect(response.body.message).toBe('Failed to delete product');
        });
    });
});
