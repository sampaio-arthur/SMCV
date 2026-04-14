# Pages — Containers de Estado e Logica

## Padrao Container

Cada page segue o padrao Container:
- Gerencia estado com `useState` (items, loading)
- Chama services no `useEffect` (mount) e nos handlers
- Trata erros com `try/catch` e exibe feedback via `useToast`
- Passa dados e callbacks para o componente presenter

## Arquivos

| Arquivo | Rota | Titulo | Descricao |
|---------|------|--------|-----------|
| `ExamplePage.jsx` | `/example` | Example Page | Template de referencia |
| `UserPage.jsx` | `/user` | Usuarios | Gerenciamento de usuarios |
| `UserProfilePage.jsx` | `/user-profile` | Perfis de Usuario | Curriculo e credenciais SMTP |
| `CampaignPage.jsx` | `/campaign` | Campanhas | Campanhas de envio de curriculo |
| `ContactPage.jsx` | `/contact` | Contatos | Contatos vinculados a campanhas |

## Fluxo de dados típico

### Carregamento inicial
useEffect → loadItems() → service.getAll() → setItems(data)

### Create (sem re-fetch)
handler → const novoItem = await service.create(data)
       → setItems(prev => [...prev, novoItem])
       → toast.success()

### Update (sem re-fetch)
handler → const itemAtualizado = await service.update(id, data)
       → setItems(prev => prev.map(x => x.id === id ? itemAtualizado : x))
       → toast.success()

### Delete (sem re-fetch)
handler → await service.remove(id)
       → setItems(prev => prev.filter(x => x.id !== id))
       → toast.success()

### Exceções onde re-fetch é necessário
- handleSendEmails (CampaignPage): altera status de múltiplos contatos no backend
- handleUploadResume (UserProfilePage): somente se endpoint retornar 204 sem body

### Princípio
A atualização local só ocorre APÓS sucesso da API.
Se a API falhar, o catch é acionado e setItems nunca executa.

## Particularidades

### UserProfilePage
- Carrega perfis E usuarios em paralelo (`Promise.all`)
- Passa `users` ao componente para resolver nomes

### CampaignPage
- Usa `useNavigate` para redirecionar ao clicar em "Ver contatos"
- Navega para `/contact?campaignId={id}`

### ContactPage
- Le `campaignId` de `useSearchParams`
- Se presente: carrega contatos da campanha (`getAllByCampaignId`)
- Se ausente: carrega todos os contatos (`getAll`)
- Carrega campanhas em paralelo para popular select

## Mensagens de erro (pt-BR)

Todas as mensagens seguem o padrao:
- Create: "Nao foi possivel criar [recurso]. Verifique os campos e tente novamente."
- Update: "Nao foi possivel atualizar [recurso]. Verifique sua conexao."
- Delete: "Nao foi possivel excluir [recurso]."
- Load: "Nao foi possivel carregar [recurso]. Verifique se a API esta em execucao."

## Regras

- **NENHUMA** page renderiza HTML de tabela ou formulario diretamente
- **NENHUMA** page chama Axios diretamente — sempre via service
- Toda page exibe LoadingSpinner durante carregamento
- Toda page usa `useToast` para feedback
- Funções de carregamento usadas em useEffect devem ser envoltas em useCallback
- handlers de create/update/delete usam atualização de estado local — loadItems() não é chamado neles

## Padrão useCallback

Funções de carregamento usadas em useEffect DEVEM ser envoltas em useCallback:

```jsx
const loadItems = useCallback(async () => {
  // lógica de carregamento
}, []);

useEffect(() => {
  loadItems();
}, [loadItems]);
```

useCallback([]) garante referência estável — o useEffect roda apenas uma vez.

Exceção: useEffects com IIFEs internas ou que modificam suas próprias dependências
devem usar eslint-disable-next-line com comentário justificando o motivo.
