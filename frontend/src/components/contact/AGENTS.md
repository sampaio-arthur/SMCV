# Contact Component

## Arquivo: `ContactComponent.jsx`

Componente presenter para o recurso **Contato**.

## Props

| Prop | Tipo | Descricao |
|------|------|-----------|
| `items` | `Array` | Lista de contatos |
| `campaigns` | `Array` | Lista de campanhas (para o select) |
| `onCreate` | `Function(data)` | Callback para criar contato |
| `onUpdate` | `Function(id, data)` | Callback para atualizar contato |
| `onDelete` | `Function(id)` | Callback para excluir contato |
| `defaultCampaignId` | `string` | ID da campanha pre-selecionada (via query param) |

## Tabela — Colunas

| Coluna | Campo | Tipo visual |
|--------|-------|-------------|
| Empresa | `companyName` | Texto |
| E-mail | `email` | Texto |
| Regiao | `region` | Texto ("—" se null) |
| Status do envio | `emailStatus` | StatusBadge |
| Enviado em | `emailSentAt` | Data ou "Nao enviado" |
| Acoes | — | Pencil, Trash2 |

## StatusBadge — Mapa de cores

| Status | Label | Cor |
|--------|-------|-----|
| Pending | Pendente | gray |
| Sent | Enviado | green |
| Failed | Falhou | red |

## Formulario — Campos

| Label | Campo | Input type | Required |
|-------|-------|-----------|----------|
| Campanha * | `campaignId` | select | Sim |
| Nome da empresa * | `companyName` | text | Sim |
| E-mail * | `email` | email | Sim |
| Regiao | `region` | text | Nao |

**Campos read-only (nunca no form):** `emailStatus`, `emailSentAt`

## Filtros

- **Por status**: Tabs (Todos, Pendente, Enviado, Falhou)
- **Por texto**: SearchInput com debounce — busca por empresa ou e-mail

## Contadores

Resumo no topo: "X pendente(s), Y enviado(s), Z falhou(aram)"

## Comportamento

- `campaignId` read-only no formulario de edicao
- `defaultCampaignId` pre-seleciona campanha se vindo de `/contact?campaignId=X`
- ConfirmDialog antes de excluir
- Empty state contextual (filtros ativos vs lista vazia)

## Heuristicas de Nielsen aplicadas

- H1: StatusBadge, data de envio, contadores por status
- H5: `type="email"`, status nao editavel
- H6: Resumo visual de contadores
- H7: Busca com debounce, filtro por status
- H9: Empty state contextual
