# Install `dotnet-clean-arch` skill into Claude Code

This folder ships a Claude Code **skill** that scaffolds a new .NET 8
Clean Architecture project from this template repository.

## What is a Claude Code skill?

A skill is a markdown file (`SKILL.md`) with YAML frontmatter that Claude
Code loads at startup. When the user's request matches the skill's
`description` or `triggers`, Claude invokes it automatically — like a
reusable playbook.

Skills live under one of these directories:

| Scope | Path | When to use |
|---|---|---|
| **User (global)** | `~/.claude/skills/<name>/SKILL.md` | You want this skill available in every project on your machine |
| **Project-local** | `<repo>/.claude/skills/<name>/SKILL.md` | You want the skill only when Claude is running inside this repo |

This repo keeps a copy at `.claude/skills/dotnet-clean-arch/SKILL.md` so
the template and its skill travel together.

## Install (user scope — recommended)

```powershell
# Windows
New-Item -ItemType Directory -Force -Path "$env:USERPROFILE\.claude\skills\dotnet-clean-arch" | Out-Null
Copy-Item -Path ".\.claude\skills\dotnet-clean-arch\SKILL.md" `
          -Destination "$env:USERPROFILE\.claude\skills\dotnet-clean-arch\SKILL.md" -Force
```

```bash
# macOS / Linux
mkdir -p ~/.claude/skills/dotnet-clean-arch
cp .claude/skills/dotnet-clean-arch/SKILL.md ~/.claude/skills/dotnet-clean-arch/SKILL.md
```

Restart any running `claude` session so it rescans the skills directory.

## Install (project scope)

No extra step — Claude Code auto-discovers `.claude/skills/**/SKILL.md`
inside the current working directory when launched from this repo.

## Verify it loaded

Start Claude Code in this repo and type:
```
/help
```
You should see `dotnet-clean-arch` in the skill list, or run it directly:
```
/dotnet-clean-arch
```

Claude will prompt for the 3 arguments (`TARGET_DIR`, `NEW_NAME`,
`GITHUB_OWNER`) and then clone + rename + re-init git for you.

## Usage examples

**Interactive:**
```
You: /dotnet-clean-arch
Claude: What's TARGET_DIR? NEW_NAME? GITHUB_OWNER?
You: D:\Projects\OrderService, OrderService, acme-corp
Claude: [clones, renames, re-inits git, prints next steps]
```

**One-shot with natural language:**
```
You: new .NET project at D:\Projects\Billing called BillingService, owner quangnx99
```

**Offline / air-gapped** (copy from a local clone instead of GitHub):
```powershell
$env:DOTNET_CLEAN_ARCH_TEMPLATE = "D:\Projects\dotnet-starter"
# then run the skill — it will detect the env var and use Copy-Item instead of git clone
```

## Uninstall

```powershell
Remove-Item -Recurse -Force "$env:USERPROFILE\.claude\skills\dotnet-clean-arch"
```

## Updating the skill

When the template evolves (new stack choices, changed defaults):

1. Edit `.claude/skills/dotnet-clean-arch/SKILL.md` in this repo
2. Commit + push
3. On each machine: re-run the install copy command above

Keeping the skill inside the repo means anyone who clones the template
can install the skill in one command without visiting a separate gist or
repo.
