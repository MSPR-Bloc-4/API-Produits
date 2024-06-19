const mongoose = require('mongoose');

const productDetailsSchema = new mongoose.Schema({
    price: {
        type: String,
        required: true,
    },
    description: {
        type: String,
        required: true,
    },
});

const productSchema = new mongoose.Schema({
    name: {
        type: String,
        required: true,
    },
    details: {
        type: productDetailsSchema,
        required: true,
    },
    stock: {
        type: Number,
        required: true,
        default: 0,
    },
    createdAt: {
        type: Date,
        default: Date.now,
    },
});

const Product = mongoose.model('Product', productSchema);

module.exports = Product;
