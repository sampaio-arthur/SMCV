# UserProfile Component

## Arquivo: `UserProfileComponent.jsx`

Componente presenter para o recurso **Perfil de Usuario**.

## Props

| Prop | Tipo | Descricao |
|------|------|-----------|
| `items` | `Array` | Lista de perfis |
| `users` | `Array` | Lista de usuarios (para popular o select) |
| `onCreate` | `Function(data)` | Callback para criar perfil |
| `onUpdate` | `Function(id, data)` | Callback para atualizar perfil |
| `onDelete` | `Function(id)` | Callback para excluir perfil |

## Tabela — Colunas

| Coluna | Campo | Tipo visual |
|--------|-------|-------------|
| Usuario | `userId` (resolvido para nome) | Texto |
| Curriculo | `resumeFileName` | Texto ("Nenhum" se null) |
| SMTP Host | `smtpHost` | Texto ("Nao configurado" se null) |
| SMTP E-mail | `smtpEmail` | Texto ("Nao configurado" se null) |
| Acoes | — | Icones Pencil e Trash2 |

## Formulario — Campos

| Label | Campo | Input type | Required | Helper text |
|-------|-------|-----------|----------|-------------|
| Usuario * | `userId` | select | Sim | Dropdown com usuarios |
| Nome do curriculo | `resumeFileName` | text | Nao | "Nome do arquivo PDF enviado" |
| Servidor SMTP | `smtpHost` | text | Nao | — |
| Porta SMTP | `smtpPort` | number (1-65535) | Nao | "Portas comuns: 587 (TLS), 465 (SSL)" |
| E-mail SMTP | `smtpEmail` | email | Nao | — |
| Senha SMTP | `smtpPassword` | password | Nao | "Use uma App Password do Gmail" |

## Comportamento

- `userId` e read-only no formulario de edicao
- Campos SMTP opcionais (usuario configura depois)
- Layout grid 2 colunas para campos SMTP
- ConfirmDialog antes de excluir
- Resolucao de userId → nome do usuario na tabela

## Heuristicas de Nielsen aplicadas

- H2: Labels em portugues, helper texts explicativos
- H5: `type="number"` com min/max, `type="password"`, `type="email"`
- H6: Placeholders com exemplos reais
- H10: Helper text em campos SMTP
