const express = require('express');
const mongoose = require('mongoose');
const dotenv = require('dotenv');

dotenv.config();

const app = express();
const PORT = process.env.PORT || 5000;
const MONGODB_URI = process.env.MONGODB_URI;

// Connexion à la base de données au démarrage
const connectDatabase = async () => {
    try {
        await mongoose.connect(MONGODB_URI, {
            dbName: 'paye-ton-kawa',
        });
        console.log('MongoDB connected...');
    } catch (error) {
        console.error('Connection to MongoDB failed:', error.message);
        process.exit(1); // Quitte le processus Node.js en cas d'échec de la connexion
    }
};

// Middleware pour parser les requêtes JSON
app.use(express.json());

// Routes des produits
const productRoutes = require('./routes/products');
app.use('/products', productRoutes);

// Démarrage du serveur
const startServer = async () => {
    try {
        await connectDatabase();
        app.listen(PORT, () => {
            console.log(`Server listening on port ${PORT}`);
        });
    } catch (error) {
        console.error('Error starting the server:', error.message);
        process.exit(1); // Quitte le processus Node.js en cas d'erreur au démarrage du serveur
    }
};

startServer();

// Fonction pour fermer la connexion à la base de données
const closeDatabase = async () => {
    await mongoose.connection.close();
};

module.exports = { app, connectDatabase, closeDatabase };
