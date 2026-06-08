# GiftShuffle Backend

Clean Architecture (.NET 10) API — Secret Santa / Amigo Invisible.

## Project structure

```
GiftShuffle.Domain/             → Entities only (Friend, ShuffleHistory). Zero external packages.
GiftShuffle.Application/        → Interfaces, DTOs (records), Service implementations.
GiftShuffle.Infraestructure/    → EF Core SQLite, Identity (AppUser), MailKit, JWT, Repository impls.
GiftShuffle.Api/                → Controllers, Program.cs (DI + middleware).
```

Dependency direction: `Api → Infrastructure → Application → Domain`

## Commands

- `dotnet build GiftShuffle.slnx` — build all 4 projects (uses `.slnx` format, not `.sln`)
- `dotnet run --project GiftShuffle` — run API
- SQLite DB auto-created on first run via `db.Database.EnsureCreated()`

## Auth

- `AppUser : IdentityUser<Guid>` (adds Name + LastName)
- JWT tokens config in `appsettings.json` under `Jwt` section
- Public: `POST /api/auth/register`, `POST /api/auth/login`
- Protected (JWT required): all `/api/friends/*`, `POST /api/shuffle`

## API endpoints

| Method | Path | Auth |
|--------|------|------|
| POST | /api/auth/register | No |
| POST | /api/auth/login | No |
| GET | /api/friends | JWT |
| POST | /api/friends | JWT |
| PUT | /api/friends/{id} | JWT |
| DELETE | /api/friends/{id} | JWT |
| POST | /api/shuffle | JWT |

## Shuffle algorithm

Fisher-Yates shuffle + circular rotation (nobody gives to self). Excludes any (giver, receiver) pair from `ShuffleHistory` to prevent repeats. Retries up to 100 attempts.

## DB

- SQLite via `EF Core Sqlite`. Connection string key: `DefaultConnection`.
- Tables created automatically: `Friends`, `ShuffleHistories`, plus Identity tables (`AspNetUsers`, etc.).

## Known quirks

- `.slnx` file format (XML-based solution file, not `.sln`).
- Infrastructure project name is `Infraestructure` (Spanish spelling, missing 'u').
- Infrastructure uses `<FrameworkReference Include="Microsoft.AspNetCore.App" />` to access ASP.NET Core framework types.

## Custom skill

`Skills/csharp.md` — 952-line C# architecture skill. Registered in `opencode.json` at repo root.

## Permission config

File edits allowed only under `GiftShuffleBackend/**` (configured in `opencode.json` at repo root).
