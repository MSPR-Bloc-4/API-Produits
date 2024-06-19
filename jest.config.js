module.exports = {
    testEnvironment: 'node',
    coverageDirectory: 'coverage',
    collectCoverage: true,
    coverageThreshold: {
        global: {
            branches: 100,
            functions: 100,
            lines: 100,
            statements: 100,
        },
    },
    testMatch: ['**/__tests__/**/*.test.js'],
    collectCoverageFrom: [
        '**/*.js',
        '!**index.js',
        '!**/node_modules/**',
        '!**/coverage/**',
        '!**/jest.config.js', 
        '!**/__tests__/**', 
      ],
      coveragePathIgnorePatterns: [
        '/index.js',
        '/node_modules/',
        '/coverage/',
        '/jest.config.js',
        '/__tests__/',
      ],
};
