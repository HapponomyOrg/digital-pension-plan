const inquirer = require('inquirer');

module.exports = {
    prompter: function(cz, commit) {
        inquirer.prompt([
            {
                type: 'list',
                name: 'type',
                message: 'Select the type of change:',
                choices: [
                    { value: 'feat', name: 'feat:     A new feature' },
                    { value: 'fix', name: 'fix:      A bug fix' },
                    { value: 'docs', name: 'docs:     Documentation changes' },
                    { value: 'style', name: 'style:    Code style changes' },
                    { value: 'refactor', name: 'refactor: Code refactoring' },
                    { value: 'perf', name: 'perf:     Performance improvements' },
                    { value: 'test', name: 'test:     Adding tests' },
                    { value: 'chore', name: 'chore:    Build/tooling changes' },
                    { value: 'revert', name: 'revert:   Revert a commit' },
                    { value: 'wip', name: 'wip:      Work in progress' }
                ]
            },
            {
                type: 'input',
                name: 'scope',
                message: 'Scope (press enter to skip):'
            },
            {
                type: 'input',
                name: 'subject',
                message: 'Short description:',
                validate: function(input) {
                    if (input.length === 0) {
                        return 'Description is required';
                    }
                    if (input.length > 100) {
                        return 'Description must be 100 characters or less';
                    }
                    return true;
                }
            },
            {
                type: 'input',
                name: 'body',
                message: 'Longer description (press enter to skip):'
            },
            {
                type: 'confirm',
                name: 'isBreaking',
                message: 'Are there any breaking changes?',
                default: false
            },
            {
                type: 'input',
                name: 'breaking',
                message: 'Describe the breaking changes:',
                when: function(answers) {
                    return answers.isBreaking;
                }
            },
            {
                type: 'confirm',
                name: 'noChangelog',
                message: 'Add [no-changelog]?',
                default: false
            }
        ]).then(function(answers) {
            const scope = answers.scope ? `(${answers.scope})` : '';
            const subject = answers.subject;
            const body = answers.body || '';
            const breaking = answers.breaking ? `BREAKING CHANGE: ${answers.breaking}` : '';
            const noChangelog = answers.noChangelog ? '[no-changelog]' : '';

            const header = `${answers.type}${scope}: ${subject} ${noChangelog}`.trim();
            const fullMessage = [header, body, breaking].filter(Boolean).join('\n\n');

            commit(fullMessage);
        });
    }
};