const supertest = require('supertest');
const mongoose = require('mongoose');
const { app, connectDatabase, closeDatabase, startServer } = require('../server');

let server;

describe('Server tests', () => {
    let connectMock;
    let closeMock;

    beforeAll(() => {
        jest.spyOn(console, 'log').mockImplementation(() => {});
        jest.spyOn(console, 'error').mockImplementation(() => {});
        connectMock = jest.spyOn(mongoose, 'connect').mockImplementation(() => Promise.resolve());
        closeMock = jest.spyOn(mongoose.connection, 'close').mockImplementation(() => Promise.resolve());
    });

    beforeEach(async () => {
        // Démarrez le serveur sur un port aléatoire pour éviter les conflits
        const port = Math.floor(Math.random() * (65535 - 1024) + 1024);
        server = await startServer(port);
    });

    afterEach(async () => {
        if (server) {
            await new Promise((resolve, reject) => {
                server.close(err => (err ? reject(err) : resolve()));
            });
        }
    });

    afterAll(async () => {
        await closeDatabase();
        jest.restoreAllMocks();
    });

    it('should connect to MongoDB and start the server', async () => {
        await connectDatabase();
        expect(connectMock).toHaveBeenCalled();

        // Vérifiez une route qui n'existe pas pour obtenir un statut 404
        const response = await supertest(app).get('/non-existent-route');
        expect(response.status).toBe(404);
    });

    it('should handle MongoDB connection failure', async () => {
        connectMock.mockImplementationOnce(() => Promise.reject(new Error('Connection failed')));
        const processExitMock = jest.spyOn(process, 'exit').mockImplementation(() => { throw new Error('process.exit called') });

        await expect(connectDatabase()).rejects.toThrow('process.exit called');
        expect(console.error).toHaveBeenCalledWith('Connection to MongoDB failed:', 'Connection failed');
        expect(processExitMock).toHaveBeenCalledWith(1);

        processExitMock.mockRestore();
    });

    it('should close the MongoDB connection', async () => {
        await closeDatabase();
        expect(closeMock).toHaveBeenCalled();
    });

    it('should start the server and listen on the specified port', async () => {
        const serverStart = jest.spyOn(app, 'listen').mockImplementation((port, callback) => callback());

        const port = Math.floor(Math.random() * (65535 - 1024) + 1024);
        await startServer(port);

        expect(serverStart).toHaveBeenCalledWith(port, expect.any(Function));
        expect(console.log).toHaveBeenCalledWith(expect.stringContaining('MongoDB connected...'));
        expect(console.log).toHaveBeenCalledWith(expect.stringContaining(`Server listening on port ${port}`));
    });

    it('should handle server start failure', async () => {
        connectMock.mockImplementationOnce(() => Promise.reject(new Error('Start failed')));
        const processExitMock = jest.spyOn(process, 'exit').mockImplementation(() => { throw new Error('process.exit called') });

        await expect(startServer()).rejects.toThrow('process.exit called');
        expect(console.error).toHaveBeenCalledWith('Connection to MongoDB failed:', 'Start failed');
        expect(processExitMock).toHaveBeenCalledWith(1);

        processExitMock.mockRestore();
    });
});
