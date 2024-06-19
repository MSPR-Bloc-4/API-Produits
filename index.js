const { startServer } = require('./server');

// Vérification pour exécution directe
if (require.main === module) {
    startServer().catch(err => {
        console.error('Failed to start server:', err.message);
        process.exit(1);
    });
}
