# SMCV — Job Prospector

Sistema de prospecção automatizada de empregos via e-mail. Busca contatos de empresas por nicho/região usando a API Hunter.io, permite gerenciamento de campanhas e disparo de e-mails com currículo anexado.

## Problema que resolve

Automatiza o processo manual de buscar contatos de empresas, organizar campanhas de e-mail e enviar currículos em massa — reduzindo o tempo de prospecção de horas para minutos.

## Pré-requisitos

- .NET 8 SDK
- PostgreSQL 15+
- Node.js 18+
- Conta Hunter.io (free tier: [criar conta](https://hunter.io/users/sign_up))
- Conta SMTP (Gmail, Outlook, etc.)
- Docker e Docker Compose (opcional)

## Configuração do Banco

Criar o banco PostgreSQL:

```sql
CREATE DATABASE jobprospector;
```

## Configuração

Copie o arquivo de exemplo e preencha com suas credenciais:

```bash
cp .env.example .env
```

Edite o `.env` com seus valores reais. **Nunca commite o `.env`.**

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `DB_HOST` | Host do PostgreSQL | `localhost` |
| `DB_PORT` | Porta do PostgreSQL | `5432` |
| `DB_NAME` | Nome do banco | `jobprospector` |
| `DB_USER` | Usuário do banco | `postgres` |
| `DB_PASSWORD` | Senha do banco | `change-me` |
| `POSTGRES_DB` | Nome do banco (Docker) | `jobprospector` |
| `POSTGRES_USER` | Usuário (Docker) | `postgres` |
| `POSTGRES_PASSWORD` | Senha (Docker) | `change-me` |
| `HUNTER_API_KEY` | Chave da API Hunter.io | `your_key_here` |
| `SMTP_HOST` | Servidor SMTP | `smtp.gmail.com` |
| `SMTP_PORT` | Porta SMTP | `587` |
| `SMTP_SENDER_EMAIL` | E-mail remetente | `your@email.com` |
| `SMTP_SENDER_PASSWORD` | Senha ou App Password | `your_app_password` |
| `SMTP_SENDER_NAME` | Nome do remetente | `Job Prospector` |

## Execução com Docker

```bash
docker-compose up -d --build
```

- Frontend: http://localhost:3000
- Backend API: http://localhost:8080
- Swagger: http://localhost:8080/swagger

## Execução Local

### Backend

```bash
cd backend
dotnet ef database update   # aplica migrations
dotnet run                  # API em :8080
```

### Frontend

```bash
cd frontend
npm install
npm run dev                 # Vite em :5173
```

## Endpoints

Documentação interativa disponível em `/swagger`.

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/campaigns` | Listar campanhas |
| GET | `/api/campaigns/{id}` | Detalhes da campanha com contatos |
| POST | `/api/campaigns` | Criar campanha (multipart/form-data) |
| PUT | `/api/campaigns/{id}` | Atualizar campanha |
| DELETE | `/api/campaigns/{id}` | Excluir campanha |
| POST | `/api/campaigns/{id}/send` | Disparar e-mails da campanha |
| GET | `/api/campaigns/{id}/export-csv` | Exportar contatos em CSV |
| GET | `/api/contacts/{id}` | Detalhes do contato |
| GET | `/api/contacts/campaign/{campaignId}` | Contatos de uma campanha |
| POST | `/api/contacts` | Criar contato manualmente |
| DELETE | `/api/contacts/{id}` | Excluir contato |
| POST | `/api/contacts/search` | Buscar contatos via Hunter.io |
| GET | `/api/emaillogs/contact/{contactId}` | Log de e-mail por contato |
| GET | `/api/emaillogs/campaign/{campaignId}` | Logs de e-mail por campanha |

## Diagrama de Entidades

```
Campaign (1) ──── (N) Contact (1) ──── (1) EmailLog
   │                      │                    │
   ├─ Id                  ├─ Id                ├─ Id
   ├─ Niche               ├─ CompanyName       ├─ ContactId (UNIQUE)
   ├─ Region              ├─ Email             ├─ Status
   ├─ ResumeFileName      ├─ Domain            ├─ SentAt
   ├─ ResumeFilePath      ├─ ContactName       ├─ ErrorMessage
   ├─ EmailSubject        ├─ Position          └─ CreatedAt
   ├─ EmailBody           ├─ Source
   ├─ Status              ├─ CampaignId (FK)
   └─ CreatedAt           └─ CreatedAt
```

## Stack

- **Backend:** ASP.NET Core 8, EF Core, MediatR (CQRS), AutoMapper, FluentValidation, MailKit
- **Frontend:** React 19, Vite, Tailwind CSS
- **Banco:** PostgreSQL 15+
- **Infra:** Docker Compose

## Limitações Conhecidas

- Envio de e-mails é síncrono (sem fila/background job)
- Hunter.io free tier: 25 buscas/mês
- Sem autenticação/autorização implementada
- Sem rate limiting no envio de e-mails
