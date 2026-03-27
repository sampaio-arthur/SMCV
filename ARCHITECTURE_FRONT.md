# Architecture — Frontend

## Overview

React 19 Single Page Application (SPA) built with Vite. Follows a feature-oriented folder structure with clear separation between pages, components, services, and state management.

---

## Technologies

| Technology | Version | Purpose |
|---|---|---|
| React | 19 | UI library |
| Vite | 8 | Build tool and dev server |
| React Router DOM | 7 | Client-side routing |
| Axios | 1.x | HTTP client |
| Tailwind CSS | 4 | Utility-first CSS framework |
| Lucide React | latest | Icon library |
| nginx | alpine | Static file server (Docker) |

---

## Folder Structure

```
frontend/src/
├── components/         # Reusable UI and feature components
│   ├── example/        # Feature components (ExampleComponent, etc.)
│   └── ui/             # Generic UI primitives (Modal, Toast, ConfirmDialog)
├── pages/              # Page-level components — one per route
├── layouts/            # App shell / wrapper components (sidebar, nav)
├── routes/             # Centralised route configuration
├── services/           # API communication layer (Axios wrappers)
├── hooks/              # Custom React hooks
├── contexts/           # React Context providers (global state)
├── store/              # Global state management placeholder
├── utils/              # Pure utility functions (formatDate, truncate, etc.)
└── main.jsx            # App entry point
```

---

## Data Flow

```
API (backend)
    │
    ▼
Service (exampleService.js)     — Axios calls, returns plain JS objects
    │
    ▼
Page (ExamplePage.jsx)          — owns state, calls Service, passes props down
    │
    ▼
Component (ExampleComponent.jsx) — renders UI, raises events via callbacks
    │
    ▼
Toast / UI feedback             — useToast() hook → ToastContext
```

---

## Routing

Routes are defined centrally in `src/routes/index.jsx`:

```jsx
<Routes>
  <Route element={<MainLayout />}>        // Persistent shell (sidebar + outlet)
    <Route path="/" element={<Navigate to="/example" />} />
    <Route path="/example" element={<ExamplePage />} />
  </Route>
</Routes>
```

To add a new page:
1. Create `src/pages/YourPage.jsx`
2. Add a `<Route>` in `src/routes/index.jsx`
3. Add a nav entry in `src/layouts/MainLayout.jsx` → `navItems` array

---

## State Management

### Local state
`useState` / `useReducer` inside pages and components for ephemeral UI state (form fields, loading flags, modals).

### Global state
Light global state is provided through React Context (`ToastContext`). For heavier needs, `src/store/index.js` is the integration point for Zustand, Redux Toolkit, or Jotai.

### Server state
No dedicated server-state library is included. Consider **TanStack Query** (`@tanstack/react-query`) for caching, background refetching, and optimistic updates.

---

## Service Layer (`src/services/`)

Each resource has its own service file that wraps Axios:

```js
// exampleService.js
const api = axios.create({ baseURL: `${API_URL}/api/example` });

export const getAll  = () => api.get('/').then(r => r.data);
export const create  = (data) => api.post('/', data).then(r => r.data);
export const update  = (id, data) => api.put(`/${id}`, data).then(r => r.data);
export const remove  = (id) => api.delete(`/${id}`).then(r => r.data);
```

The `VITE_API_URL` environment variable overrides the default `http://localhost:8080`.

---

## Notifications

`ToastContext` exposes `{ success, error, info }` helpers available anywhere via `useToast()`:

```jsx
const toast = useToast();
toast.success('Saved!');
toast.error('Something went wrong.');
```

Toasts auto-dismiss after 4 seconds.

---

## Environment Variables

Prefix with `VITE_` to expose to the browser bundle:

```env
# .env.local (local development)
VITE_API_URL=http://localhost:8080
```

---

## Patterns Adopted

- **Feature folders**: components, services, and pages are colocated by domain, not by file type.
- **Container / Presenter**: pages (containers) own state and call services; components (presenters) receive props and emit callbacks.
- **Custom Hooks**: reusable logic extracted to `src/hooks/` (e.g. `useToast`).
- **Centralised routing**: all route definitions live in one file, making the app's navigation easy to audit.
- **Service abstraction**: HTTP calls are never made inside components — always through a service module.
