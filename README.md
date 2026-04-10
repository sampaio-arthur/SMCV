# SMCV — Job Prospector

Sistema de prospecção automatizada de empregos via e-mail. Permite gerenciamento de campanhas, contatos e disparo de e-mails com currículo anexado.

## Problema que resolve

Automatiza o processo de organizar campanhas de e-mail e enviar currículos em massa — reduzindo o tempo de prospecção de horas para minutos.

## Pré-requisitos

- .NET 8 SDK
- PostgreSQL 15+
- Node.js 18+
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

Autenticação via sessão (email + senha). Endpoints de auth:

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/auth/register` | Registrar novo usuário |
| POST | `/api/auth/login` | Login (inicia sessão) |
| POST | `/api/auth/logout` | Logout (limpa sessão) |
| GET | `/api/auth/me` | Dados do usuário logado |
| GET | `/api/users` | Listar usuários |
| GET | `/api/users/{id}` | Detalhes do usuário |
| POST | `/api/users` | Criar usuário |
| PUT | `/api/users/{id}` | Atualizar usuário |
| DELETE | `/api/users/{id}` | Excluir usuário |
| GET | `/api/userprofiles` | Listar perfis |
| GET | `/api/userprofiles/{id}` | Detalhes do perfil |
| GET | `/api/userprofiles/user/{userId}` | Perfil por usuário |
| POST | `/api/userprofiles` | Criar perfil |
| POST | `/api/userprofiles/{id}/upload-resume` | Upload de currículo (PDF) |
| PUT | `/api/userprofiles/{id}` | Atualizar perfil |
| DELETE | `/api/userprofiles/{id}` | Excluir perfil |
| GET | `/api/campaigns` | Listar campanhas |
| GET | `/api/campaigns/{id}` | Detalhes da campanha com contatos |
| POST | `/api/campaigns` | Criar campanha (JSON) |
| PUT | `/api/campaigns/{id}` | Atualizar campanha |
| DELETE | `/api/campaigns/{id}` | Excluir campanha |
| POST | `/api/campaigns/{id}/send` | Disparar e-mails da campanha |
| GET | `/api/campaigns/{id}/export-csv` | Exportar contatos em CSV |
| GET | `/api/contacts/{id}` | Detalhes do contato |
| GET | `/api/contacts/campaign/{campaignId}` | Contatos de uma campanha |
| POST | `/api/contacts` | Criar contato manualmente |
| PUT | `/api/contacts/{id}` | Atualizar contato |
| DELETE | `/api/contacts/{id}` | Excluir contato |
| GET | `/api/emaillogs/contact/{contactId}` | Log de e-mail por contato |
| GET | `/api/emaillogs/campaign/{campaignId}` | Logs de e-mail por campanha |

## Diagrama de Entidades

```
User (1) ──────────────── (1) UserProfile
  │                              UserId (FK, UNIQUE)
  └── (1) ──────────── (N) Campaign
                               UserId (FK)
                                  └── (1) ── (N) Contact
                                                  CampaignId (FK)
                                                     └── (1) ── (0..1) EmailLog
                                                                        ContactId (FK, UNIQUE)

User                UserProfile          Campaign
├─ Id               ├─ Id                ├─ Id
├─ Name             ├─ UserId (FK)       ├─ UserId (FK)
├─ Email (UNIQUE)   ├─ ResumeFilePath    ├─ Name
├─ PasswordHash     └─ CreatedAt         ├─ Niche
└─ CreatedAt                             ├─ Region
                                         ├─ EmailSubject
Contact              EmailLog            ├─ EmailBody
├─ Id                ├─ Id               ├─ Status
├─ CampaignId (FK)   ├─ ContactId (FK)   └─ CreatedAt
├─ CompanyName       ├─ ErrorMessage
├─ Email             └─ CreatedAt
├─ EmailStatus
├─ EmailSentAt
└─ CreatedAt
```

## Stack

- **Backend:** ASP.NET Core 8, EF Core, MediatR (CQRS), AutoMapper, FluentValidation, MailKit, BCrypt
- **Frontend:** React 19, Vite, Tailwind CSS
- **Banco:** PostgreSQL 15+
- **Auth:** Sessão simples (email + senha com BCrypt)
- **Infra:** Docker Compose

## Limitações Conhecidas

- Envio de e-mails é síncrono (sem fila/background job)
- Sem rate limiting no envio de e-mails
