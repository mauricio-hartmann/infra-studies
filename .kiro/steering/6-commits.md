---
inclusion: always
---

# Git Commit Conventions

Never commit automatically; always wait for user instruction to both commit and push changes to the remote repository.
First, show the commit message and asks the user for approval or changes. Only when user approves the commit message you can commit. After commit, asks the user if you can push.
When writing or generating commit messages, the agent must strictly follow the *Conventional Commits* format:

## Format
<type>(<scope>): <subject>

## Rules
1. **Types:** Use only `feat`, `fix`, `docs`, `style`, `refactor`, `test`, or `chore`.
2. **Imperative Mood:** The subject line must be in the imperative mood (e.g., "Add feature," not "Added feature").
3. **Length:** Keep the first line strictly under 50 characters.
4. **Line break:** Add a line break after the first line.
5. **Body:** Always include a body with bullet points detailing what was changed and the reason, in the case of complex changes.