# Hooks — Logica Reutilizavel

## Arquivos

| Arquivo | Descricao | Retorno |
|---------|-----------|---------|
| `useToast.js` | Acesso ao ToastContext | `{ success, error, info, toasts, removeToast }` |
| `useDebounce.js` | Debounce de valor | `debouncedValue` |
| `useConfirm.js` | Gerenciamento de ConfirmDialog | `{ isOpen, confirm, cancel, dialogProps }` |

## useToast

```jsx
const toast = useToast();
toast.success('Salvo com sucesso!');
toast.error('Erro ao salvar.');
toast.info('Informacao.');
```

## useDebounce

```jsx
const [search, setSearch] = useState('');
const debouncedSearch = useDebounce(search, 300);

useEffect(() => {
  // Executado apenas quando o usuario para de digitar
  fetchResults(debouncedSearch);
}, [debouncedSearch]);
```

## useConfirm

Encapsula a logica de abrir/fechar ConfirmDialog:

```jsx
const { confirm, dialogProps } = useConfirm();

<button onClick={() => confirm(() => handleDelete(id))}>Excluir</button>
<ConfirmDialog {...dialogProps} title="Confirmar" message="Tem certeza?" />
```

- `confirm(callback)` abre o dialog e armazena o callback
- Ao clicar "Confirmar", executa o callback e fecha
- Ao clicar "Cancelar" ou fechar o modal, apenas fecha

## Regras

- Hooks nao fazem chamadas HTTP
- Hooks nao contem logica de dominio
- Prefixo `use` obrigatorio
