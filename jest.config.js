module.exports = {
    testEnvironment: 'node',
    coverageDirectory: 'coverage',
    collectCoverage: true,
    collectCoverageFrom: ['server.js'],
    coverageThreshold: {
        global: {
            branches: 100,
            functions: 100,
            lines: 100,
            statements: 100,
        },
    },
};
