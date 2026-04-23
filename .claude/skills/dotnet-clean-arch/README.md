# Install `dotnet-clean-arch` skill into Claude Code

This folder ships a Claude Code **skill** that scaffolds a new .NET 8
Clean Architecture project using `dotnet new clean-arch`.

The skill is a thin wrapper — the real work is done by Microsoft's
`dotnet new` template engine, configured by `.template.config/template.json`
at the repo root.

## What is a Claude Code skill?

A skill is a markdown file (`SKILL.md`) with YAML frontmatter that Claude
Code loads at startup. When the user's request matches the skill's
`description` or `triggers`, Claude invokes it automatically.

| Scope | Path | When to use |
|---|---|---|
| **User (global)** | `~/.claude/skills/<name>/SKILL.md` | Available in every project on your machine |
| **Project-local** | `<repo>/.claude/skills/<name>/SKILL.md` | Only when Claude runs inside this repo |

## One-time setup on a new machine

### Step 1 — Install the dotnet template

**Option A — From a local clone (recommended for active development):**
```bash
git clone https://github.com/quangnx99/dotnet-clean-arch.git ~/src/dotnet-clean-arch
dotnet new install ~/src/dotnet-clean-arch
```

**Option B — From NuGet (once the package is published):**
```bash
dotnet new install DotnetCleanArch.Template
```

Verify:
```bash
dotnet new list clean-arch
```

### Step 2 — Install the Claude skill

**User scope (recommended):**
```powershell
# Windows
New-Item -ItemType Directory -Force -Path "$env:USERPROFILE\.claude\skills\dotnet-clean-arch" | Out-Null
Copy-Item ".\.claude\skills\dotnet-clean-arch\SKILL.md" `
          "$env:USERPROFILE\.claude\skills\dotnet-clean-arch\SKILL.md" -Force
```

```bash
# macOS / Linux
mkdir -p ~/.claude/skills/dotnet-clean-arch
cp .claude/skills/dotnet-clean-arch/SKILL.md ~/.claude/skills/dotnet-clean-arch/SKILL.md
```

**Project scope** — no-op. Claude auto-discovers `.claude/skills/**/SKILL.md`
inside the current working directory.

Restart any running `claude` session so it rescans the skills directory.

## Verify

```
/dotnet-clean-arch
```
Claude should ask for `NEW_NAME`, `TARGET_DIR`, `GITHUB_OWNER`, then run
`dotnet new clean-arch ...` and `git init`.

## Usage examples

**Interactive:**
```
You: /dotnet-clean-arch
Claude: NEW_NAME? TARGET_DIR? GITHUB_OWNER?
You: OrderService, D:\Projects\OrderService, acme-corp
Claude: [runs dotnet new, git init, prints next steps]
```

**One-shot:**
```
You: new .NET project at D:\Projects\Billing called BillingService, owner quangnx99
```

**Without the skill** — pure CLI works the same way:
```bash
dotnet new clean-arch -n OrderService --githubOwner acme-corp -o ./OrderService
cd OrderService && git init -b main && git add -A && git commit -m "init"
```

## Updating

Template engine first:
```bash
cd ~/src/dotnet-clean-arch
git pull
dotnet new install . --force
```

Skill file second (only if SKILL.md changed):
```powershell
Copy-Item ".\.claude\skills\dotnet-clean-arch\SKILL.md" `
          "$env:USERPROFILE\.claude\skills\dotnet-clean-arch\SKILL.md" -Force
```

## Uninstall

```bash
dotnet new uninstall DotnetCleanArch.Template          # if installed via NuGet
dotnet new uninstall ~/src/dotnet-clean-arch           # if installed from path

# remove the skill file
rm -rf ~/.claude/skills/dotnet-clean-arch              # bash
Remove-Item -Recurse -Force "$env:USERPROFILE\.claude\skills\dotnet-clean-arch"  # PowerShell
```
