# Services — Camada de Comunicacao com a API

## Padrao

Cada recurso tem seu proprio arquivo service que encapsula todas as chamadas HTTP via Axios. Nenhum outro arquivo da aplicacao faz chamadas HTTP diretamente.

## Arquivos

| Arquivo | Rota base | Operacoes |
|---------|-----------|-----------|
| `exampleService.js` | `/api/example` | getAll, getById, create, update, remove |
| `userService.js` | `/api/user` | getAll, getById, create, update, remove |
| `userProfileService.js` | `/api/userprofile` | getAll, getById, getByUserId, create, update, remove |
| `campaignService.js` | `/api/campaign` | getAll, getById, create, update, remove, updateStatus |
| `contactService.js` | `/api/contact` | getAll, getById, getAllByCampaignId, create, update, remove |

## Estrutura padrao de um service

```js
import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8080';

const api = axios.create({
  baseURL: `${API_URL}/api/{recurso}`,
  headers: { 'Content-Type': 'application/json' },
});

export const getAll = async () => {
  const response = await api.get('/');
  return response.data;
};
```

## Operacoes extras

| Service | Operacao | Endpoint |
|---------|----------|----------|
| `userProfileService` | `getByUserId(userId)` | `GET /api/userprofile/user/{userId}` |
| `campaignService` | `updateStatus(id, status)` | `PUT /api/campaign/{id}/status` |
| `contactService` | `getAllByCampaignId(campaignId)` | `GET /api/contact/campaign/{campaignId}` |

## Regras

- Todo service usa `axios.create` com `baseURL` propria
- Funcoes SEMPRE retornam `response.data` (nao o wrapper do Axios)
- Nomenclatura: `getAll`, `getById`, `create`, `update`, `remove`
- **NUNCA** chamar Axios fora desta pasta
- Variavel de ambiente: `VITE_API_URL` (default: `http://localhost:8080`)
