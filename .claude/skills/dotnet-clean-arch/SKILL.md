---
name: dotnet-clean-arch
description: >
  Bootstrap a new .NET 8 Clean Architecture project from the
  DotnetCleanArch.Template via the `dotnet new` CLI. Use when the user
  asks to start a new .NET project, create a .NET Clean Architecture
  scaffold, or mentions /dotnet-clean-arch.
triggers:
  - /dotnet-clean-arch
  - "new .NET project"
  - "clean architecture scaffold"
  - "dotnet clean arch"
---

# Skill: dotnet-clean-arch

Scaffold a production-ready .NET 8 Clean Architecture project using the
official `dotnet new` template engine. No file-by-file rename, no
PowerShell hacks — Microsoft's template engine handles namespaces,
filenames, sln entries, and content substitution natively.

## Arguments (prompt if missing)

| Argument | Description | Example |
|---|---|---|
| `NEW_NAME` | PascalCase project name (becomes namespace + sln name) | `OrderService` |
| `TARGET_DIR` | Absolute path for the new project | `D:\Projects\OrderService` |
| `GITHUB_OWNER` | GitHub username/org for badges + remote | `acme-corp` |

## Steps

### Step 0 — Gather arguments

If any argument is missing, ask the user. Do NOT proceed with placeholders.

### Step 1 — Ensure the template is installed

Check if `clean-arch` short name is available:
```bash
dotnet new list clean-arch
```

If missing, install ONE of the following:

**A. From local clone (preferred during development):**
```bash
git clone https://github.com/quangnx99/dotnet-clean-arch.git /tmp/dotnet-clean-arch
dotnet new install /tmp/dotnet-clean-arch
```

**B. From NuGet (once published):**
```bash
dotnet new install DotnetCleanArch.Template
```

### Step 2 — Scaffold the project

```bash
dotnet new clean-arch \
  --name <NEW_NAME> \
  --githubOwner <GITHUB_OWNER> \
  --output <TARGET_DIR>
```

The template engine automatically:
- Replaces `DotnetCleanArch` → `<NEW_NAME>` in every file, namespace, sln entry
- Replaces `dotnet-clean-arch` → `<kebab-case of NEW_NAME>` (e.g. `OrderService` → `order-service`)
- Replaces `quangnx99` → `<GITHUB_OWNER>`
- Renames every file/folder containing `DotnetCleanArch`
- Excludes `bin/`, `obj/`, `.git/`, `openspec/`, `logs/`, `.idea/`, `.vs/`
- Runs `dotnet restore` post-action

### Step 3 — Initialise git

```bash
cd <TARGET_DIR>
git init -b main
git add -A
git commit -m "chore: scaffold from dotnet-clean-arch template"
```

### Step 4 — Print next steps

```
Project scaffolded at <TARGET_DIR>

Next steps:
  1. cd <TARGET_DIR>
  2. Copy .env.example to .env and fill in secrets:
       copy .env.example .env         (Windows)
       cp .env.example .env           (bash)
  3. Start dependencies:
       docker compose up -d postgres redis
  4. Apply database migrations:
       dotnet ef database update \
         --project src\<NEW_NAME>.Infrastructure \
         --startup-project src\<NEW_NAME>.Api
  5. Run the API:
       dotnet run --project src\<NEW_NAME>.Api
  6. Open Swagger UI:
       http://localhost:5080/swagger
  7. Create the GitHub repo and push:
       gh repo create <GITHUB_OWNER>/<kebab-name> --public --source=. --push
```

## Updating / uninstalling the template

```bash
# Update from local clone after `git pull`
dotnet new install /tmp/dotnet-clean-arch --force

# Uninstall
dotnet new uninstall /tmp/dotnet-clean-arch
# or
dotnet new uninstall DotnetCleanArch.Template
```
