# Architecture — Backend

## Overview

ASP.NET Core 8 REST API following a layered architecture (Controller → Service → Repository → Database). The codebase is domain-agnostic and ready to be extended with any business logic.

---

## Technologies

| Technology | Version | Purpose |
|---|---|---|
| ASP.NET Core | 8.0 | Web framework |
| Entity Framework Core | 8.0 | ORM / database migrations |
| Npgsql EF Provider | 8.0 | PostgreSQL driver for EF |
| Swashbuckle (Swagger) | 6.5 | Auto-generated API docs |
| PostgreSQL | latest | Relational database |

---

## Folder Structure

```
backend/
├── Controllers/        # HTTP entry points — maps routes to Service calls
├── Services/           # Business logic — interfaces + implementations
├── Repositories/       # Data access — interfaces + implementations
├── Entities/           # EF Core domain models (tables)
├── DTOs/               # Request/Response contracts (no entity exposure)
├── Data/               # AppDbContext — EF Core configuration
├── Utils/              # Shared utilities (ApiResponse wrapper, etc.)
├── Program.cs          # App bootstrap, DI registration, middleware pipeline
├── appsettings.json    # Configuration (connection string, logging)
└── Dockerfile          # Container build instructions
```

---

## Layer Responsibilities

### Controller
- Receives HTTP requests and validates route/body parameters.
- Delegates all logic to the Service layer — contains **no business rules**.
- Returns appropriate HTTP status codes (200, 201, 204, 404, etc.).
- Example: `ExampleController.cs`

### Service
- Orchestrates business logic.
- Receives DTOs from the Controller and returns DTOs back.
- Calls the Repository for data access; never uses `DbContext` directly.
- Example: `ExampleService.cs` implementing `IExampleService`.

### Repository
- Sole responsible for database interactions.
- Uses `AppDbContext` (EF Core) to query and persist data.
- Returns domain `Entities`; the Service maps them to DTOs.
- Example: `ExampleRepository.cs` implementing `IExampleRepository`.

### Entity
- Plain C# class mapped to a database table by EF Core.
- No business logic — only data structure.
- Example: `Example.cs` → table `Examples`.

### DTO (Data Transfer Object)
- Defines the contract between API and consumers.
- **Request DTO**: data accepted from the client (POST/PUT body).
- **Response DTO**: data returned to the client — no internal fields exposed.

---

## Request Flow

```
HTTP Request
    │
    ▼
Controller          — validates input, calls Service
    │
    ▼
Service             — applies business rules, maps Entity ↔ DTO
    │
    ▼
Repository          — executes SQL via EF Core
    │
    ▼
AppDbContext        — EF Core ORM
    │
    ▼
PostgreSQL Database
```

---

## Configuration

### Connection String (`appsettings.json`)
```json
"DefaultConnection": "Host={DB_HOST};Port=5432;Database=app;Username=user;Password=password"
```
The `{DB_HOST}` placeholder is replaced at runtime by the `DB_HOST` environment variable (injected by Docker Compose). Falls back to `localhost` for local development.

### CORS
Policy `AllowAll` permits any origin, method, and header. **Restrict this in production** using `AllowSpecificOrigins`.

### Swagger
Enabled only in the `Development` environment. Access at `http://localhost:8080/swagger`.

---

## Dependency Injection

Registrations in `Program.cs`:

```csharp
builder.Services.AddScoped<IExampleRepository, ExampleRepository>();
builder.Services.AddScoped<IExampleService, ExampleService>();
```

Use `Scoped` lifetime for services that depend on `DbContext` (one instance per HTTP request).

---

## Adding a New Resource

1. Create `Entities/YourEntity.cs`
2. Add `DbSet<YourEntity>` to `AppDbContext`
3. Run `dotnet ef migrations add AddYourEntity && dotnet ef database update`
4. Create `DTOs/YourEntityRequestDto.cs` + `YourEntityResponseDto.cs`
5. Create `Repositories/IYourEntityRepository.cs` + implementation
6. Create `Services/IYourEntityService.cs` + implementation
7. Create `Controllers/YourEntityController.cs`
8. Register in `Program.cs`

---

## Patterns Adopted

- **Interface Segregation**: every Service and Repository has a corresponding interface, enabling easy mocking and testing.
- **DTO pattern**: entities are never serialised directly to the client; DTOs control the public contract.
- **Async/Await throughout**: all I/O operations are non-blocking.
- **Dependency Injection**: constructor injection, managed by the .NET DI container.
- **Repository pattern**: isolates data access from business logic.
