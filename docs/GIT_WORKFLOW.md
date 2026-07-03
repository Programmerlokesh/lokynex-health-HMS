# Git Workflow — Lokynex Health

## Branching
- main: always stable, deployable
- feature/<name>: one feature/module per branch

## Commit Convention
feat: | fix: | docs: | chore: | refactor: | test:

## Workflow
1. git checkout -b feature/<name>
2. Work, commit often with conventional commits
3. git checkout main && git merge feature/<name>
4. git branch -d feature/<name>