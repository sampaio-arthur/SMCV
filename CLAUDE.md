# CLAUDE.md

## Stack

- **Backend:** ASP.NET Core 8 + EF Core + PostgreSQL — namespace `SMCV`
- **Frontend:** React 19 + Vite + Tailwind CSS
- **Infra:** Docker Compose (db, backend, frontend)

## Commands

```bash
# Docker (full stack)
docker-compose up -d            # start all
docker-compose up -d --build    # rebuild + start
docker-compose down             # stop

# Backend (from backend/)
dotnet run                      # API on :8080
dotnet build
dotnet ef migrations add Name
dotnet ef database update

# Frontend (from frontend/)
npm run dev                     # Vite on :5173
npm run build
npm run lint
```

## URLs

| Service | Docker | Local |
|---------|--------|-------|
| Frontend | :3000 | :5173 |
| Backend API | :8080 | :8080 |
| Swagger | :8080/swagger | :8080/swagger |

## Backend Architecture

Arquitetura CQRS: Controller -> MediatR Handler -> Repository -> PostgreSQL.

**Documentacao por camada em `backend/AGENTS.md`** — ponto de entrada com mapa de navegacao para cada pasta. Cada subpasta tem seu proprio `AGENTS.md` com regras, proibicoes, padroes de codigo e snippets especificos daquela camada:

| Tarefa | Arquivo |
|--------|---------|
| Endpoint HTTP | `backend/Controllers/AGENTS.md` |
| Handler CQRS (logica de negocio) | `backend/Features/AGENTS.md` |
| Entidade de banco / Enums | `backend/Domain/AGENTS.md` |
| DTO / Interfaces / Mappings | `backend/Application/AGENTS.md` |
| DbContext / Repositories / APIs externas | `backend/Infrastructure/AGENTS.md` |
| Result / Excecoes | `backend/Common/AGENTS.md` |
| Program.cs / DI / pipeline | `backend/PROGRAM_AGENTS.md` |

## Adding a New Resource

### Backend
1. `Domain/Entities/Xxx.cs` + `DbSet` em `AppDbContext` + migration
2. `Application/DTOs/XxxRequestDto.cs` + `XxxResponseDto.cs`
3. `Application/Interfaces/IXxxRepository.cs`
4. `Infrastructure/Repositories/XxxRepository.cs`
5. `Features/Xxx/Commands/` + `Features/Xxx/Queries/` (handlers CQRS)
6. `Controllers/XxxController.cs`
7. Registrar no DI em `Program.cs` (`AddScoped`)

### Frontend
1. `services/xxxService.js`
2. `pages/XxxPage.jsx`
3. `components/xxx/XxxComponent.jsx`
4. Rota em `routes/index.jsx`
5. Nav em `layouts/MainLayout.jsx` → `navItems`

## Frontend Structure

```
src/
├── components/ui/      # Modal, Toast, ConfirmDialog
├── components/example/  # ExampleComponent (table + form)
├── pages/              # XxxPage — state + service calls
├── layouts/            # MainLayout — sidebar + Outlet
├── routes/             # AppRoutes (index.jsx)
├── services/           # Axios wrappers para /api/xxx
├── hooks/              # useToast
├── contexts/           # ToastContext
├── store/              # Global state (Zustand/Redux/Jotai)
├── utils/              # formatDate, truncate, sleep, get
└── main.jsx            # BrowserRouter + ToastProvider + App
```

API base URL: `VITE_API_URL` (default `http://localhost:8080`).

## Environment

Connection string via env vars: `DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASSWORD`.
Auto-migrate habilitado no startup (`Program.cs`).
