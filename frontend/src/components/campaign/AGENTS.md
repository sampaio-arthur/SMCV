# Campaign Component

## Arquivo: `CampaignComponent.jsx`

Componente presenter para o recurso **Campanha**. O mais complexo do sistema.

## Props

| Prop | Tipo | Descricao |
|------|------|-----------|
| `items` | `Array` | Lista de campanhas |
| `onCreate` | `Function(data)` | Callback para criar campanha |
| `onUpdate` | `Function(id, data)` | Callback para atualizar campanha |
| `onDelete` | `Function(id)` | Callback para excluir campanha |
| `onViewContacts` | `Function(campaignId)` | Callback para navegar aos contatos |

## Tabela — Colunas

| Coluna | Campo | Tipo visual |
|--------|-------|-------------|
| Nome | `name` | Texto |
| Nicho | `niche` | Texto |
| Regiao | `region` | Texto |
| Status | `status` | StatusBadge |
| Contatos | `contacts.length` / `contactCount` | Numero |
| Criado em | `createdAt` | Data formatada |
| Acoes | — | Eye, Pencil, Trash2 |

## StatusBadge — Mapa de cores

| Status | Label | Cor | Extra |
|--------|-------|-----|-------|
| Draft | Rascunho | gray | — |
| Ready | Pronto | blue | — |
| Sending | Enviando | yellow | `animate-pulse` |
| Completed | Concluido | green | — |

## Formulario — Campos

| Label | Campo | Input type | Required |
|-------|-------|-----------|----------|
| Nome da campanha * | `name` | text | Sim |
| Nicho * | `niche` | text | Sim |
| Regiao * | `region` | text | Sim |
| Assunto do e-mail * | `emailSubject` | text | Sim |
| Corpo do e-mail * | `emailBody` | textarea (rows=4) | Sim |

## Filtro por status

Tabs no topo da tabela: Todas, Rascunho, Pronto, Enviando, Concluido. Filtragem client-side.

## Restricoes por status

| Status | Editar | Excluir |
|--------|--------|---------|
| Draft | Sim | Sim |
| Ready | Sim | Sim |
| Sending | **NAO** | **NAO** |
| Completed | **NAO** | Sim |

## Heuristicas de Nielsen aplicadas

- H1: StatusBadge com cores, contagem de contatos, pulse no "Enviando"
- H3: ConfirmDialog antes de excluir
- H5: Bloqueio de edicao/exclusao por status, textarea para corpo de e-mail
- H7: Filtro por status via tabs, acoes inline
- H8: Corpo do e-mail nao aparece na tabela
