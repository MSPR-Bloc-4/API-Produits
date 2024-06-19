const mongoose = require('mongoose');
const { MongoMemoryServer } = require('mongodb-memory-server');
const Product = require('../models/Product'); // Adjust the path as per your folder structure

let mongoServer;

// Connect to the in-memory database before running tests
beforeAll(async () => {
    mongoServer = await MongoMemoryServer.create();
    const mongoUri = mongoServer.getUri();

    await mongoose.connect(mongoUri, {
        useNewUrlParser: true,
        useUnifiedTopology: true,
    });
});

// Clear the database and close the connection after all tests
afterAll(async () => {
    await mongoose.disconnect();
    await mongoServer.stop();
});

// Clear the database after each test
afterEach(async () => {
    await Product.deleteMany();
});

describe('Product Model Tests', () => {
    it('should create a new product', async () => {
        const productData = {
            name: 'Test Product',
            details: {
                price: '19.99',
                description: 'Test description',
            },
            stock: 10,
        };

        const product = new Product(productData);
        const savedProduct = await product.save();

        expect(savedProduct._id).toBeDefined();
        expect(savedProduct.name).toBe(productData.name);
        expect(savedProduct.details.price).toBe(productData.details.price);
        expect(savedProduct.details.description).toBe(productData.details.description);
        expect(savedProduct.stock).toBe(productData.stock);
        expect(savedProduct.createdAt).toBeDefined();
    });

    it('should require name field', async () => {
        const productData = {
            details: {
                price: '19.99',
                description: 'Test description',
            },
            stock: 10,
        };

        let error;
        try {
            await new Product(productData).save();
        } catch (err) {
            error = err;
        }

        expect(error).toBeInstanceOf(mongoose.Error.ValidationError);
        expect(error.errors['name']).toBeDefined();
    });

    it('should require details.price field', async () => {
        const productData = {
            name: 'Test Product',
            details: {
                description: 'Test description',
            },
            stock: 10,
        };

        let error;
        try {
            await new Product(productData).save();
        } catch (err) {
            error = err;
        }

        expect(error).toBeInstanceOf(mongoose.Error.ValidationError);
        expect(error.errors['details.price']).toBeDefined();
    });

    // Add more tests as needed for other fields and scenarios
});
