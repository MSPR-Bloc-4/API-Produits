const express = require('express');
const mongoose = require('mongoose');
const dotenv = require('dotenv');

dotenv.config();

const app = express();

const MONGODB_URI = process.env.MONGODB_URI;

const connectDatabase = () => {
    return mongoose.connect(MONGODB_URI)
        .then(() => console.log('MongoDB connected...'))
        .catch(err => console.log('Connection failed'));
};

app.use(express.json());

app.get('/', (req, res) => {
    res.send('Hello from your REST API!');
});

const closeDatabase = async () => {
    await mongoose.connection.close();
};

module.exports = { app, connectDatabase, closeDatabase };
