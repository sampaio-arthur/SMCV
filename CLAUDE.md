# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Domain-agnostic full-stack template. Provides the complete scaffolding (backend layers, frontend structure, Docker environment) with generic `Example` resources that are replaced when building a real application.

**Stack:** ASP.NET Core 8 + EF Core + PostgreSQL (backend) / React 19 + Vite + Tailwind CSS (frontend)

See `ARCHITECTURE_BACKEND.md` and `ARCHITECTURE_FRONT.md` for detailed architecture documentation.

---

## Development Commands

### Full stack via Docker (recommended)
```bash
docker-compose up -d          # Build and start all services (db, backend, frontend)
docker-compose down           # Stop all services
docker-compose up -d --build  # Rebuild images after code changes
```

URLs when running via Docker:
- Frontend: http://localhost:3000
- Backend API: http://localhost:8080
- Swagger: http://localhost:8080/swagger

### Database only
```bash
docker-compose up -d db       # Start only PostgreSQL on port 5432
```

### Backend (from `backend/`)
```bash
dotnet run                    # Start API on http://localhost:8080
dotnet build                  # Build only
dotnet ef migrations add Name # Add EF migration (requires db running)
dotnet ef database update     # Apply migrations
```

### Frontend (from `frontend/`)
```bash
npm run dev                   # Start Vite dev server (http://localhost:5173)
npm run build                 # Production build
npm run lint                  # ESLint check
npm run preview               # Preview production build
```

---

## Backend Structure (`backend/`)

```
backend/
├── Controllers/        # ExampleController — HTTP routes
├── Services/           # IExampleService + ExampleService — business logic
├── Repositories/       # IExampleRepository + ExampleRepository — data access
├── Entities/           # Example — EF Core entity (maps to DB table)
├── DTOs/               # ExampleRequestDto + ExampleResponseDto
├── Data/               # AppDbContext — EF Core DbContext
├── Utils/              # ApiResponse<T> — generic response wrapper
├── Program.cs          # DI registration, middleware pipeline
├── appsettings.json    # Config — connection string uses {DB_HOST} placeholder
└── Dockerfile
```

**API endpoint:** `GET|POST /api/example` and `GET|PUT|DELETE /api/example/{id}`

**Connection string** uses the `DB_HOST` env var (set to `db` in Docker, `localhost` locally).

**First-time setup:** After starting the database, run migrations:
```bash
cd backend
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## Frontend Structure (`frontend/src/`)

```
src/
├── components/
│   ├── example/        # ExampleComponent (table + inline form)
│   └── ui/             # Modal, Toast, ToastContainer, ConfirmDialog
├── pages/              # ExamplePage — owns state, calls services
├── layouts/            # MainLayout — sidebar + <Outlet>
├── routes/             # AppRoutes — central route config (index.jsx)
├── services/           # exampleService.js — Axios wrapper for /api/example
├── hooks/              # useToast — convenience hook over ToastContext
├── contexts/           # ToastContext — global toast notifications
├── store/              # Global state placeholder (Zustand/Redux/Jotai)
├── utils/              # Pure helpers: formatDate, truncate, sleep, get
└── main.jsx            # Entry point — BrowserRouter + ToastProvider + App
```

**API base URL** defaults to `http://localhost:8080`. Override with `VITE_API_URL` env var.

---

## Adding a New Resource

### Backend
1. `Entities/YourEntity.cs` → add `DbSet<YourEntity>` to `AppDbContext` → run migration
2. `DTOs/YourEntityRequestDto.cs` + `YourEntityResponseDto.cs`
3. `Repositories/IYourEntityRepository.cs` + `YourEntityRepository.cs`
4. `Services/IYourEntityService.cs` + `YourEntityService.cs`
5. `Controllers/YourEntityController.cs`
6. Register in `Program.cs`

### Frontend
1. `services/yourEntityService.js`
2. `pages/YourEntityPage.jsx`
3. `components/yourEntity/YourEntityComponent.jsx`
4. Add route in `routes/index.jsx`
5. Add nav entry in `layouts/MainLayout.jsx` → `navItems`
