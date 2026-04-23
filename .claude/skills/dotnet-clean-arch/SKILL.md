---
name: dotnet-clean-arch
description: >
  Scaffold a complete production-ready .NET 8 Clean Architecture backend
  (Domain / Application / Infrastructure / Api + tests + Docker + CI)
  from the DotnetCleanArch.Template via `dotnet new clean-arch`. Use
  whenever the user asks to create / start / build / scaffold a new .NET
  backend, REST API, web API, microservice, or "backend app for X". Also
  triggered by /dotnet-clean-arch or natural-language requests in
  Vietnamese ("tạo backend", "tạo dự án .net", "scaffold .net").
triggers:
  - /dotnet-clean-arch
  - "new .NET project"
  - "new dotnet project"
  - "new .NET backend"
  - "create .NET backend"
  - "scaffold .NET"
  - "clean architecture scaffold"
  - "dotnet clean arch"
  - "tạo backend"
  - "tạo dự án .net"
  - "tạo project .net"
  - "scaffold .net"
---

# Skill: dotnet-clean-arch

Scaffold a complete .NET 8 Clean Architecture backend in one shot. The
underlying engine is `dotnet new clean-arch`, which renames namespaces,
file names, sln entries, badge URLs, and runs `dotnet restore`
automatically. After scaffolding the skill does a build verification so
the user gets a project that compiles green on first checkout.

## Argument inference (do NOT just ask blindly)

The user typically writes things like:
- `/dotnet-clean-arch tôi muốn tạo backend app cho todo-app`
- `new .NET backend for a blog`
- `scaffold a payment API called BillingService at D:\Projects\Billing`

Derive arguments from the request. **Only ask for what you cannot
reasonably infer.**

| Argument | How to derive | Fallback |
|---|---|---|
| `NEW_NAME` | Convert the noun phrase to PascalCase. `todo-app` → `TodoApp`. `blog` → `BlogApi`. `payment service` → `PaymentService`. If the user gave an explicit name (`called X`, `tên là X`), trust it as-is. | Ask user (with your suggestion: "I'll call it `TodoApp` — okay?") |
| `TARGET_DIR` | If absolute path mentioned, use it. Otherwise default to `./<NEW_NAME>` in current working directory. | `./<NEW_NAME>` (no ask) |
| `GITHUB_OWNER` | Use it if mentioned. Otherwise check `git config user.name` or `gh api user --jq .login`. | Skip the `--githubOwner` flag (template default is `your-username`, user can sed later) |

When in doubt about `NEW_NAME`, ask once with a concrete suggestion. Never
ask three separate questions if you can infer 2/3 from context.

## Steps

### Step 0 — Parse and confirm

Echo back to the user in their language what you understood:

> "Tôi sẽ tạo project **TodoApp** ở `./TodoApp` (owner mặc định
> `your-username`, đổi sau cũng được). OK không?"

Wait for confirm. If the user pushes back, adjust and re-confirm.

### Step 1 — Ensure the template is installed

```bash
dotnet new list clean-arch
```

If it returns "No templates found":

```bash
# Try NuGet first (lightweight)
dotnet new install DotnetCleanArch.Template 2>/dev/null || \
  ( git clone https://github.com/quangnx99/dotnet-clean-arch.git /tmp/dotnet-clean-arch && \
    dotnet new install /tmp/dotnet-clean-arch )
```

### Step 2 — Scaffold

```bash
dotnet new clean-arch \
  --name <NEW_NAME> \
  --githubOwner <GITHUB_OWNER>   # omit flag entirely if not provided \
  --output <TARGET_DIR>
```

The template engine will:
- Rename `DotnetCleanArch` → `<NEW_NAME>` in every namespace, filename, sln entry
- Rewrite `dotnet-clean-arch` → kebab-case of `<NEW_NAME>` (e.g. `todo-app`)
- Rewrite badge owner `quangnx99` → `<GITHUB_OWNER>` if provided
- Exclude `bin/`, `obj/`, `.git/`, `openspec/`, `logs/`, `.idea/`, `.vs/`
- Run `dotnet restore` as a post-action

### Step 3 — Initialise git

```bash
cd <TARGET_DIR>
git init -b main
git add -A
git commit -m "chore: scaffold from dotnet-clean-arch template"
```

### Step 4 — Verify build (REQUIRED)

```bash
dotnet build --nologo -v q
```

If build fails: surface the error, do NOT pretend success. Common causes:
- Missing .NET 8 SDK → tell user to install it
- Network issue during restore → suggest `dotnet restore --force-evaluate`

### Step 5 — Print next steps

Output (in the user's language):

```
✅ Project <NEW_NAME> scaffolded at <TARGET_DIR>
   Build: PASS · Tests pending DB

Next steps:
  1. cd <TARGET_DIR>
  2. cp .env.example .env  (and fill in REDIS_PASSWORD if any)
  3. docker compose up -d postgres redis
  4. dotnet ef database update \
       --project src/<NEW_NAME>.Infrastructure \
       --startup-project src/<NEW_NAME>.Api
  5. dotnet run --project src/<NEW_NAME>.Api
  6. Open Swagger: http://localhost:5080/swagger
  7. (optional) push to GitHub:
       gh repo create <owner>/<kebab-name> --public --source=. --push
```

## Anti-patterns

- ❌ Asking the user "What's NEW_NAME?" when they said "todo-app"
- ❌ Asking for `TARGET_DIR` when current directory is empty/sane
- ❌ Asking for `GITHUB_OWNER` when the user only wants a quick local prototype
- ❌ Skipping the build verification step
- ❌ Reporting success when `dotnet build` failed
