# UI Components — Primitivos Reutilizaveis

## Arquivos

| Arquivo | Descricao | Props |
|---------|-----------|-------|
| `Modal.jsx` | Modal generico com overlay, Escape, botao X | `isOpen, onClose, title, children, size` |
| `Toast.jsx` | Notificacao individual (success/error/info) | `type, message, onClose` |
| `ToastContainer.jsx` | Container fixo que renderiza a fila de toasts | — (consome `useToast`) |
| `ConfirmDialog.jsx` | Dialog de confirmacao (usa Modal internamente) | `isOpen, onClose, onConfirm, title, message` |
| `StatusBadge.jsx` | Badge colorido generico por status | `status, config` |
| `EmptyState.jsx` | Estado vazio padronizado com icone e acao | `icon, title, description, actionLabel, onAction` |
| `LoadingSpinner.jsx` | Spinner de carregamento centralizado | `message` (default: "Carregando...") |
| `SearchInput.jsx` | Input de busca com debounce integrado | `onChange, placeholder, delay` |

## StatusBadge — Mapa de configuracao

O componente recebe um objeto `config` que mapeia valores de status para aparencia:

```js
const config = {
  Draft: { label: 'Rascunho', bg: 'bg-gray-100', text: 'text-gray-600' },
  Ready: { label: 'Pronto', bg: 'bg-blue-100', text: 'text-blue-700' },
  Sending: { label: 'Enviando', bg: 'bg-yellow-100', text: 'text-yellow-700', extra: 'animate-pulse' },
  Completed: { label: 'Concluido', bg: 'bg-green-100', text: 'text-green-700' },
};
```

Campos: `label` (texto exibido), `bg` (classe Tailwind de fundo), `text` (classe de texto), `extra` (classes opcionais).

## EmptyState — Uso

```jsx
<EmptyState
  title="Nenhum item encontrado"
  description="Crie o primeiro item para comecar."
  actionLabel="Novo Item"
  onAction={() => setShowForm(true)}
/>
```

Se `actionLabel` e `onAction` forem omitidos, o botao nao e renderizado.

## SearchInput — Debounce

O componente gerencia seu proprio estado interno. A prop `onChange` e chamada apenas apos o usuario parar de digitar por `delay` ms (default: 300ms).

```jsx
<SearchInput onChange={(term) => setSearchTerm(term)} placeholder="Buscar..." />
```

## Regras

- **NAO** alterar `Modal.jsx`, `Toast.jsx`, `ConfirmDialog.jsx`, `ToastContainer.jsx` — ja validados
- Novos primitivos devem ser genéricos (sem logica de dominio)
- Todos devem receber dados via props e nao fazer chamadas HTTP
- Seguir o padrao visual Tailwind existente (cores, espacamento, bordas)
