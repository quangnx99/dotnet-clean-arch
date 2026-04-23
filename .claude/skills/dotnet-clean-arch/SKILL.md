---
name: dotnet-clean-arch
description: >
  Bootstrap a new .NET 8 Clean Architecture project from the
  dotnet-clean-arch template repository. Use when the user asks to start
  a new .NET project, create a .NET Clean Architecture scaffold, or
  mentions /dotnet-clean-arch.
triggers:
  - /dotnet-clean-arch
  - "new .NET project"
  - "clean architecture scaffold"
  - "dotnet clean arch"
---

# Skill: dotnet-clean-arch

Bootstrap a production-ready .NET 8 Clean Architecture project by cloning
the template and renaming every occurrence of `DotnetCleanArch` to a new
project name.

## Arguments (prompt if missing)

| Argument | Description | Example |
|---|---|---|
| `TARGET_DIR` | Absolute path for the new project | `D:\Projects\MyService` |
| `NEW_NAME` | PascalCase project name (replaces `DotnetCleanArch`) | `OrderService` |
| `GITHUB_OWNER` | GitHub username or org for the new repo badges / remote | `acme-corp` |

Optional env override:
- `DOTNET_CLEAN_ARCH_TEMPLATE` — local path to a cloned template. If set,
  the skill copies from disk instead of calling `git clone`. Useful for
  offline or air-gapped environments.

## Steps

### Step 0 — Gather arguments

If any argument is missing, ask the user before proceeding. Do NOT proceed
with placeholder values.

Compute derived values:
- `NEW_NAME_KEBAB` = `NEW_NAME` converted to lowercase-kebab-case
  (e.g. `OrderService` → `order-service`)

### Step 1 — Obtain the template

**Option A — Clone from GitHub (default):**
```powershell
git clone https://github.com/quangnx99/dotnet-clean-arch.git <TARGET_DIR>
```
_(bash: same command.)_

**Option B — Local copy fallback** (when `DOTNET_CLEAN_ARCH_TEMPLATE` is set
or the clone fails):

```powershell
$source = $env:DOTNET_CLEAN_ARCH_TEMPLATE
$dest = "<TARGET_DIR>"
New-Item -ItemType Directory -Force -Path $dest | Out-Null
Copy-Item -Path "$source\*" -Destination $dest -Recurse -Exclude ".git","openspec"
```

After obtaining: verify `<TARGET_DIR>\DotnetCleanArch.sln` exists.

### Step 2 — Delete existing .git

```powershell
Remove-Item -Recurse -Force "<TARGET_DIR>\.git" -ErrorAction SilentlyContinue
```
_(bash: `rm -rf <TARGET_DIR>/.git`)_

### Step 3 — Rename files and folders

Walk `<TARGET_DIR>` **depth-first** (longest path first — avoids renaming a
parent before its children).

```powershell
Get-ChildItem -Path "<TARGET_DIR>" -Recurse -Filter "*DotnetCleanArch*" |
  Sort-Object { $_.FullName.Length } -Descending |
  ForEach-Object {
    $newName = $_.Name -replace 'DotnetCleanArch', '<NEW_NAME>'
    Rename-Item -Path $_.FullName -NewName $newName -ErrorAction Stop
  }
```

### Step 4 — Replace content in all text files

Target extensions:
`.cs`, `.csproj`, `.sln`, `.md`, `.yml`, `.yaml`, `.json`, `.props`,
`.targets`, `.editorconfig`, `.gitignore`, `.dockerignore`, `Dockerfile`,
`.env*`

Apply substitutions **in this order**:

1. `DotnetCleanArch` → `<NEW_NAME>`
2. `dotnet-clean-arch` → `<NEW_NAME_KEBAB>`
3. `quangnx99/<NEW_NAME_KEBAB>` → `<GITHUB_OWNER>/<NEW_NAME_KEBAB>`

```powershell
$extensions = @("*.cs","*.csproj","*.sln","*.md","*.yml","*.yaml",
                "*.json","*.props","*.targets","*.editorconfig",
                "*.gitignore","*.dockerignore","Dockerfile","*.env*")
Get-ChildItem -Path "<TARGET_DIR>" -Recurse -Include $extensions |
  ForEach-Object {
    $content = Get-Content $_.FullName -Raw -Encoding UTF8
    $content = $content -replace 'DotnetCleanArch', '<NEW_NAME>'
    $content = $content -replace 'dotnet-clean-arch', '<NEW_NAME_KEBAB>'
    $content = $content -replace 'quangnx99/<NEW_NAME_KEBAB>', '<GITHUB_OWNER>/<NEW_NAME_KEBAB>'
    Set-Content -Path $_.FullName -Value $content -Encoding UTF8 -NoNewline
  }
```

### Step 5 — Re-initialise git

```powershell
Set-Location <TARGET_DIR>
git init -b main
git add -A
git commit -m "chore: scaffold from dotnet-clean-arch template"
```

### Step 6 — Print next steps

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
       gh repo create <GITHUB_OWNER>/<NEW_NAME_KEBAB> --public --source=. --push
```
