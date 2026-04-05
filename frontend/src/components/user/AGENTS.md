# User Component

## Arquivo: `UserComponent.jsx`

Componente presenter para o recurso **Usuario**.

## Props

| Prop | Tipo | Descricao |
|------|------|-----------|
| `items` | `Array` | Lista de usuarios |
| `onCreate` | `Function(data)` | Callback para criar usuario |
| `onUpdate` | `Function(id, data)` | Callback para atualizar usuario |
| `onDelete` | `Function(id)` | Callback para excluir usuario |

## Tabela — Colunas

| Coluna | Campo | Tipo visual |
|--------|-------|-------------|
| Nome | `name` | Texto |
| E-mail | `email` | Texto |
| Criado em | `createdAt` | Data formatada (`formatDate`) |
| Acoes | — | Icones Pencil e Trash2 |

## Formulario — Campos

| Label | Campo | Input type | Required | Notas |
|-------|-------|-----------|----------|-------|
| Nome * | `name` | text | Sim | — |
| E-mail * | `email` | email | Sim | Validacao HTML5 |
| Senha | `password` | password | Sim (create), Nao (edit) | Vazio no edit = nao altera |

## Comportamento

- Formulario de criacao: campo Senha obrigatorio
- Formulario de edicao: campo Senha opcional (helper text explica)
- Senha nunca exibida na tabela
- ConfirmDialog antes de excluir (via `useConfirm`)
- Botao de submissao desabilitado durante envio (mostra "Salvando...")
- Empty state com botao "Novo Usuario" quando lista vazia

## Heuristicas de Nielsen aplicadas

- H1: Spinner (na page), toast apos operacoes
- H3: Botao Cancelar, ConfirmDialog
- H5: `type="email"`, `type="password"`, `required`
- H9: Empty state amigavel
