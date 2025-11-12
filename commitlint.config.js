module.exports = {
    extends: ['@commitlint/config-conventional'],
    rules: {
        'subject-case': [0],
        'subject-max-length': [2, 'always', 100]
    }
};