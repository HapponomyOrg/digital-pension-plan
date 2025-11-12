module.exports = {
    types: [
        { value: 'feat', name: 'feat:     A new feature' },
        { value: 'fix', name: 'fix:      A bug fix' },
        { value: 'docs', name: 'docs:     Documentation changes' },
        { value: 'style', name: 'style:    Code style changes' },
        { value: 'refactor', name: 'refactor: Code refactoring' },
        { value: 'perf', name: 'perf:     Performance improvements' },
        { value: 'test', name: 'test:     Adding tests' },
        { value: 'chore', name: 'chore:    Build/tooling changes' }
    ],

    scopes: [],
    allowCustomScopes: true,
    allowBreakingChanges: ['feat', 'fix'],

    skipQuestions: ['footer'],

    messages: {
        type: 'Select the type of change:',
        scope: 'Scope (press enter to skip):',
        subject: 'Short description (add [no-changelog] at end if needed):\n',
        body: 'Longer description (press enter to skip):\n',
        breaking: 'BREAKING CHANGES (press enter to skip):\n',
        confirmCommit: 'Proceed with commit?'
    },

    subjectLimit: 100
};