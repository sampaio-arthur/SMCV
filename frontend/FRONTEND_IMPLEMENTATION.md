# Documentacao da Implementacao do Frontend — SMCV

## Resumo

Implementacao completa do frontend SMCV (Sistema de Marketing por Curriculo e Vagas) com 4 novos recursos: **Usuarios**, **Perfis de Usuario**, **Campanhas** e **Contatos**. A implementacao segue o padrao Container/Presenter, as 10 heuristicas de Nielsen, e replica fielmente a estrutura do `ExamplePage`/`ExampleComponent` como modelo canonico.

---

## 1. Componentes UI Criados (`src/components/ui/`)

### StatusBadge.jsx
Badge generico colorido por status. Recebe um `status` e um `config` (mapa de cores/labels). Usado em Campaign (Draft/Ready/Sending/Completed) e Contact (Pending/Sent/Failed).

### EmptyState.jsx
Estado vazio padronizado com icone, titulo, descricao e botao de acao opcional. Garante que toda tabela vazia tenha feedback visual amigavel (H9).

### LoadingSpinner.jsx
Spinner de carregamento extraido como componente reutilizavel. Mensagem padrao "Carregando..." em portugues. Usado em todas as pages durante carregamento (H1).

### SearchInput.jsx
Input de busca com debounce integrado (default 300ms). Gerencia estado interno e so chama `onChange` apos pausa na digitacao. Usado em ContactComponent para busca por empresa/email (H7).

---

## 2. Hooks Criados (`src/hooks/`)

### useDebounce.js
Hook generico de debounce. Recebe um valor e um delay, retorna o valor debounced.

### useConfirm.js
Encapsula a logica de ConfirmDialog. Retorna `confirm(callback)` e `dialogProps` para espalhar no `<ConfirmDialog>`. Elimina boilerplate de `isOpen/pendingAction` repetido em cada componente.

---

## 3. Services Criados (`src/services/`)

Todos seguem o padrao de `exampleService.js`: instancia Axios com `baseURL` propria.

| Service | Rota base | Extras |
|---------|-----------|--------|
| `userService.js` | `/api/user` | — |
| `userProfileService.js` | `/api/userprofile` | `getByUserId(userId)` |
| `campaignService.js` | `/api/campaign` | `updateStatus(id, status)` |
| `contactService.js` | `/api/contact` | `getAllByCampaignId(campaignId)` |

---

## 4. Components Criados (`src/components/`)

Todos seguem o padrao Presenter: recebem dados via props, emitem eventos via callbacks, **sem** chamadas HTTP.

### UserComponent.jsx (`components/user/`)
- Tabela: Nome, E-mail, Criado em, Acoes
- Formulario: Nome*, E-mail* (type=email), Senha* (type=password)
- Senha obrigatoria no create, opcional no edit
- ConfirmDialog via `useConfirm`
- Empty state com `EmptyState`

### UserProfileComponent.jsx (`components/userProfile/`)
- Tabela: Usuario (nome resolvido), Curriculo, SMTP Host, SMTP E-mail, Acoes
- Formulario: Usuario* (select), Curriculo, SMTP Host, Porta (number 1-65535), E-mail, Senha
- `userId` read-only no edit
- Helper texts em campos SMTP
- Layout grid 2 colunas para campos SMTP

### CampaignComponent.jsx (`components/campaign/`)
- Tabela: Nome, Nicho, Regiao, Status (StatusBadge), Contatos, Criado em, Acoes
- Formulario: Nome*, Nicho*, Regiao*, Assunto*, Corpo (textarea rows=4)
- Filtro por status: tabs (Todas, Rascunho, Pronto, Enviando, Concluido)
- Restricoes: Sending = editar/excluir bloqueado; Completed = editar bloqueado
- Botao Eye para ver contatos da campanha
- StatusBadge com `animate-pulse` no "Enviando"

### ContactComponent.jsx (`components/contact/`)
- Tabela: Empresa, E-mail, Regiao, Status do envio (StatusBadge), Enviado em, Acoes
- Formulario: Campanha* (select), Empresa*, E-mail* (type=email), Regiao
- `campaignId` read-only no edit, pre-selecionado se vindo de query param
- Filtro por status de email (tabs)
- Busca por texto com SearchInput (debounce)
- Contadores: "X pendente(s), Y enviado(s), Z falhou(aram)"
- `emailStatus` e `emailSentAt` NUNCA no formulario (controlados pelo backend)

---

## 5. Pages Criadas (`src/pages/`)

Todas seguem o padrao Container: estado local, chamadas a services, feedback via toast.

| Page | Rota | Titulo | Descricao |
|------|------|--------|-----------|
| `UserPage.jsx` | `/user` | Usuarios | Gerenciamento de usuarios |
| `UserProfilePage.jsx` | `/user-profile` | Perfis de Usuario | Curriculo e credenciais SMTP |
| `CampaignPage.jsx` | `/campaign` | Campanhas | Campanhas de envio de curriculo |
| `ContactPage.jsx` | `/contact` | Contatos | Contatos de cada campanha |

### Particularidades
- **UserProfilePage**: `Promise.all` para carregar perfis + usuarios em paralelo
- **CampaignPage**: `useNavigate` para redirecionar a `/contact?campaignId={id}`
- **ContactPage**: `useSearchParams` para filtrar por campanha; `Promise.all` para carregar contatos + campanhas

### Mensagens de erro (todas em portugues)
- Create falhou: "Nao foi possivel criar [recurso]. Verifique os campos e tente novamente."
- Update falhou: "Nao foi possivel atualizar [recurso]. Verifique sua conexao."
- Delete falhou: "Nao foi possivel excluir [recurso]."
- Load falhou: "Nao foi possivel carregar [recurso]. Verifique se a API esta em execucao."
- Email duplicado (UserPage): "Este e-mail ja esta cadastrado." (HTTP 409)

---

## 6. Routing e Layout

### routes/index.jsx
```
/           → Redirect para /campaign
/example    → ExamplePage (mantido como referencia)
/campaign   → CampaignPage
/contact    → ContactPage
/user       → UserPage
/user-profile → UserProfilePage
```

### layouts/MainLayout.jsx — Alteracoes
- **Titulo**: "App Template" → "SMCV"
- **Subtitulo**: "Base Project" → "Marketing por Curriculo"
- **Label da nav**: "Navigation" → "Navegacao"
- **navItems**: 4 itens (Campanhas, Contatos, Usuarios, Perfis)
- **Icones**: Megaphone, Users, UserCircle, Settings (lucide-react)

---

## 7. Heuristicas de Nielsen — Evidencias

| # | Heuristica | Implementacao |
|---|-----------|---------------|
| H1 | Visibilidade do estado | LoadingSpinner, Toast (success/error), StatusBadge, contadores, "Salvando..." |
| H2 | Correspondencia com mundo real | UI em portugues, labels naturais, status traduzidos |
| H3 | Controle e liberdade | Cancelar em forms, Escape fecha modais, ConfirmDialog antes de excluir |
| H4 | Consistencia | Mesmo layout de tabela/form, mesmas cores de botao, mesmos inputs |
| H5 | Prevencao de erros | required, type=email/password/number, min/max, disabled durante submit |
| H6 | Reconhecimento | Sidebar persistente, placeholders descritivos, labels permanentes |
| H7 | Flexibilidade | Enter submete forms, filtros por status, busca com debounce |
| H8 | Design minimalista | Tabelas enxutas, espacamento Tailwind, cores neutras |
| H9 | Recuperacao de erros | Mensagens contextualizadas, empty state com acao, sem stack traces |
| H10 | Ajuda | Descricoes nas pages, tooltips nas acoes, helper texts, placeholders |

---

## 8. Estrutura final de pastas

```
frontend/src/
├── components/
│   ├── ui/
│   │   ├── Modal.jsx              (existente)
│   │   ├── Toast.jsx              (existente)
│   │   ├── ToastContainer.jsx     (existente)
│   │   ├── ConfirmDialog.jsx      (existente)
│   │   ├── StatusBadge.jsx        ← NOVO
│   │   ├── EmptyState.jsx         ← NOVO
│   │   ├── LoadingSpinner.jsx     ← NOVO
│   │   ├── SearchInput.jsx        ← NOVO
│   │   └── AGENTS.md              ← NOVO
│   ├── example/
│   │   └── ExampleComponent.jsx   (existente)
│   ├── user/
│   │   ├── UserComponent.jsx      ← NOVO
│   │   └── AGENTS.md              ← NOVO
│   ├── userProfile/
│   │   ├── UserProfileComponent.jsx ← NOVO
│   │   └── AGENTS.md              ← NOVO
│   ├── campaign/
│   │   ├── CampaignComponent.jsx  ← NOVO
│   │   └── AGENTS.md              ← NOVO
│   └── contact/
│       ├── ContactComponent.jsx   ← NOVO
│       └── AGENTS.md              ← NOVO
├── pages/
│   ├── ExamplePage.jsx            (existente)
│   ├── UserPage.jsx               ← NOVO
│   ├── UserProfilePage.jsx        ← NOVO
│   ├── CampaignPage.jsx           ← NOVO
│   ├── ContactPage.jsx            ← NOVO
│   └── AGENTS.md                  ← NOVO
├── services/
│   ├── exampleService.js          (existente)
│   ├── userService.js             ← NOVO
│   ├── userProfileService.js      ← NOVO
│   ├── campaignService.js         ← NOVO
│   ├── contactService.js          ← NOVO
│   └── AGENTS.md                  ← NOVO
├── hooks/
│   ├── useToast.js                (existente)
│   ├── useDebounce.js             ← NOVO
│   ├── useConfirm.js              ← NOVO
│   └── AGENTS.md                  ← NOVO
├── contexts/
│   └── ToastContext.jsx           (existente, nao modificado)
├── layouts/
│   └── MainLayout.jsx             ← MODIFICADO (titulo, navItems)
├── routes/
│   └── index.jsx                  ← MODIFICADO (4 novas rotas)
├── store/
│   └── index.js                   (existente)
├── utils/
│   └── index.js                   (existente)
├── App.jsx                        (existente)
├── main.jsx                       (existente)
└── index.css                      (existente)
```

---

## 9. Validacao

| Check | Resultado |
|-------|-----------|
| `npm run build` | OK — compila sem erros |
| `npm run lint` | 1 erro pre-existente (ToastContext — nao modificavel), 5 warnings pre-existentes (exhaustive-deps) |
| Arquivos criados | 22 novos (4 UI, 2 hooks, 4 services, 4 components, 4 pages, 4 AGENTS.md em components, 3 AGENTS.md em services/pages/hooks) |
| Arquivos modificados | 2 (routes/index.jsx, layouts/MainLayout.jsx) |
| Arquivos nao tocados | Modal, Toast, ConfirmDialog, ToastContainer, ToastContext, useToast, ExamplePage, ExampleComponent, exampleService, App, main, index.css, utils |

---

## 10. Resumo de arquivos criados

### Componentes (12 arquivos)
1. `src/components/ui/StatusBadge.jsx`
2. `src/components/ui/EmptyState.jsx`
3. `src/components/ui/LoadingSpinner.jsx`
4. `src/components/ui/SearchInput.jsx`
5. `src/components/user/UserComponent.jsx`
6. `src/components/userProfile/UserProfileComponent.jsx`
7. `src/components/campaign/CampaignComponent.jsx`
8. `src/components/contact/ContactComponent.jsx`

### Services (4 arquivos)
9. `src/services/userService.js`
10. `src/services/userProfileService.js`
11. `src/services/campaignService.js`
12. `src/services/contactService.js`

### Pages (4 arquivos)
13. `src/pages/UserPage.jsx`
14. `src/pages/UserProfilePage.jsx`
15. `src/pages/CampaignPage.jsx`
16. `src/pages/ContactPage.jsx`

### Hooks (2 arquivos)
17. `src/hooks/useDebounce.js`
18. `src/hooks/useConfirm.js`

### Documentacao (8 arquivos)
19. `src/components/ui/AGENTS.md`
20. `src/components/user/AGENTS.md`
21. `src/components/userProfile/AGENTS.md`
22. `src/components/campaign/AGENTS.md`
23. `src/components/contact/AGENTS.md`
24. `src/services/AGENTS.md`
25. `src/pages/AGENTS.md`
26. `src/hooks/AGENTS.md`
