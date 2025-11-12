module.exports = {
    types: [
        { value: 'feat', name: 'feat:     A new feature' },
        { value: 'fix', name: 'fix:      A bug fix' },
        { value: 'docs', name: 'docs:     Documentation only changes' },
        { value: 'style', name: 'style:    Code style changes (formatting, etc)' },
        { value: 'refactor', name: 'refactor: A code change that neither fixes a bug nor adds a feature' },
        { value: 'perf', name: 'perf:     A code change that improves performance' },
        { value: 'test', name: 'test:     Adding missing tests' },
        { value: 'chore', name: 'chore:    Changes to build process or auxiliary tools' },
        { value: 'revert', name: 'revert:   Revert to a commit' },
        { value: 'wip', name: 'wip:      Work in progress' }
    ],

    scopes: [],

    allowCustomScopes: true,
    allowBreakingChanges: ['feat', 'fix'],

    messages: {
        type: "Select the type of change that you're committing:",
        scope: 'What is the scope of this change (e.g. component or file name): (press enter to skip)',
        subject: 'Write a short, imperative tense description of the change (max 82 chars):\n',
        body: 'Provide a longer description of the change: (press enter to skip)\n',
        breaking: 'List any BREAKING CHANGES: (press enter to skip)\n',
        footer: 'List any ISSUES CLOSED by this change: (press enter to skip)\n',
        confirmCommit: 'Are you sure you want to proceed with the commit above?'
    },

    skipQuestions: [],

    subjectLimit: 82
};