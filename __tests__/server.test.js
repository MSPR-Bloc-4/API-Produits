const request = require('supertest');
const mongoose = require('mongoose');
const { app, connectDatabase, closeDatabase } = require('../server');

describe('GET /', () => {
    beforeAll(async () => {
        await connectDatabase();
    });

    afterAll(async () => {
        await closeDatabase();
    });

    it('responds with Hello from your REST API!', async () => {
        const response = await request(app).get('/');
        expect(response.status).toBe(200);
        expect(response.text).toBe('Hello from your REST API!');
    });
});

describe('Database connection', () => {
    it('logs an error when MongoDB connection fails', async () => {
        const originalEnv = process.env.MONGODB_URI;
        process.env.MONGODB_URI = 'invalid_connection_string';

        const consoleSpy = jest.spyOn(console, 'log').mockImplementation(() => {});
        await connectDatabase();
        expect(consoleSpy).toHaveBeenCalledWith(expect.stringContaining('MongoDB connected...'));

        consoleSpy.mockRestore();
        process.env.MONGODB_URI = originalEnv;
    });

    it('logs an error when mongoose connection fails', async () => {
        const originalConnect = mongoose.connect;
        mongoose.connect = jest.fn().mockImplementation(() => Promise.reject(new Error('Connection failed')));

        const consoleSpy = jest.spyOn(console, 'log').mockImplementation(() => {});
        await connectDatabase();
        expect(consoleSpy).toHaveBeenCalledWith(expect.stringContaining('Connection failed'));

        mongoose.connect = originalConnect;
        consoleSpy.mockRestore();
    });
});
