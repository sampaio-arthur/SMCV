# SMCV — Job Prospector

Sistema de prospecção automatizada de empregos via e-mail. Permite gerenciamento de campanhas, contatos e disparo de e-mails com currículo anexado.

## Problema que resolve

Automatiza o processo de organizar campanhas de e-mail e enviar currículos em massa — reduzindo o tempo de prospecção de horas para minutos.

---

## Pré-requisitos

- Docker e Docker Compose
- Conta Gmail (para envio de e-mails via SMTP)
- [Mais Informações](https://www.docker.com/products/docker-desktop/)

---

## Configuração

### 1. Criar o arquivo `.env`

```bash
cp .env.example .env
```

### 2. Configurar variáveis de banco de dados

No `.env`, defina as credenciais do PostgreSQL. Os valores de `DB_*` e `POSTGRES_*` devem ser iguais:

```env
DB_HOST=db
DB_PORT=5432
DB_NAME=jobprospector
DB_USER=postgres
DB_PASSWORD=sua_senha_segura

POSTGRES_DB=jobprospector
POSTGRES_USER=postgres
POSTGRES_PASSWORD=sua_senha_segura
```

> `DB_HOST` deve ser `db` (nome do serviço no Docker Compose).  
> `DB_PASSWORD` e `POSTGRES_PASSWORD` devem ser iguais.

### 3. Configurar e-mail SMTP (Gmail)

O sistema usa SMTP para enviar e-mails. Para usar o Gmail, você precisa gerar uma **Senha de App**:

1. Acesse [myaccount.google.com](https://myaccount.google.com)
2. Vá em **Segurança** → **Verificação em duas etapas** e ative (obrigatório)
3. Após ativar, vá em **Segurança** → **Verificação em duas etapas** → **Senhas de app** (no final da página)
4. Crie uma senha de app com nome "Job Prospector"
5. O Google vai gerar uma senha de 16 caracteres — copie **sem espaços**

```env
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_SENDER_EMAIL=seu_email@gmail.com
SMTP_SENDER_PASSWORD=abcdefghijklmnop
SMTP_SENDER_NAME=Job Prospector
```

> **Nunca use sua senha pessoal do Google.** Use sempre a Senha de App.  
> **Nunca commite o arquivo `.env`.**

### 4. Configurar o MinIO (armazenamento de currículos)

O MinIO é o servidor de armazenamento de arquivos. Sobe automaticamente junto com o Docker Compose — não é necessário instalar nada separado.

Defina as credenciais no `.env`:

```env
MINIO_ACCESS_KEY=minioadmin
MINIO_SECRET_KEY=minioadmin
MINIO_BUCKET_NAME=smcv-resumes
```

> Os valores acima são os padrão para desenvolvimento local. Em produção, use valores seguros.  
> O bucket (`smcv-resumes`) é criado automaticamente na primeira vez que um currículo é enviado.  
> A interface web do MinIO fica disponível em `http://localhost:9001` (opcional — apenas para visualização dos arquivos).

---

## Execução

```bash
docker-compose up -d --build
```

O Docker Compose sobe quatro serviços: banco de dados, MinIO, backend e frontend. As migrations são aplicadas automaticamente no startup do backend.

| Serviço       | URL                           |
| ------------- | ----------------------------- |
| Frontend      | http://localhost:3000         |
| Backend API   | http://localhost:8080         |
| Swagger       | http://localhost:8080/swagger |
| MinIO Console | http://localhost:9001         |

Para parar:

```bash
docker-compose down
```

---

## Endpoints

Documentação interativa disponível em `/swagger`.

Autenticação via sessão (email + senha).

| Método | Rota                                   | Descrição                         |
| ------ | -------------------------------------- | --------------------------------- |
| POST   | `/api/auth/register`                   | Registrar novo usuário            |
| POST   | `/api/auth/login`                      | Login (inicia sessão)             |
| POST   | `/api/auth/logout`                     | Logout (limpa sessão)             |
| GET    | `/api/auth/me`                         | Dados do usuário logado           |
| GET    | `/api/users`                           | Listar usuários                   |
| GET    | `/api/users/{id}`                      | Detalhes do usuário               |
| POST   | `/api/users`                           | Criar usuário                     |
| PUT    | `/api/users/{id}`                      | Atualizar usuário                 |
| DELETE | `/api/users/{id}`                      | Excluir usuário                   |
| GET    | `/api/userprofiles`                    | Listar perfis                     |
| GET    | `/api/userprofiles/{id}`               | Detalhes do perfil                |
| GET    | `/api/userprofiles/user/{userId}`      | Perfil por usuário                |
| POST   | `/api/userprofiles`                    | Criar perfil                      |
| POST   | `/api/userprofiles/{id}/upload-resume` | Upload de currículo (PDF)         |
| PUT    | `/api/userprofiles/{id}`               | Atualizar perfil                  |
| DELETE | `/api/userprofiles/{id}`               | Excluir perfil                    |
| GET    | `/api/campaigns`                       | Listar campanhas                  |
| GET    | `/api/campaigns/{id}`                  | Detalhes da campanha com contatos |
| POST   | `/api/campaigns`                       | Criar campanha                    |
| PUT    | `/api/campaigns/{id}`                  | Atualizar campanha                |
| DELETE | `/api/campaigns/{id}`                  | Excluir campanha                  |
| POST   | `/api/campaigns/{id}/send`             | Disparar e-mails da campanha      |
| GET    | `/api/campaigns/{id}/export-csv`       | Exportar contatos em CSV          |
| GET    | `/api/contacts/{id}`                   | Detalhes do contato               |
| GET    | `/api/contacts/campaign/{campaignId}`  | Contatos de uma campanha          |
| POST   | `/api/contacts`                        | Criar contato manualmente         |
| PUT    | `/api/contacts/{id}`                   | Atualizar contato                 |
| DELETE | `/api/contacts/{id}`                   | Excluir contato                   |
| GET    | `/api/emaillogs/contact/{contactId}`   | Log de e-mail por contato         |
| GET    | `/api/emaillogs/campaign/{campaignId}` | Logs de e-mail por campanha       |

---

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
├─ Email (UNIQUE)   ├─ ResumeFilePath*   ├─ Name
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

> `*` `ResumeFilePath` armazena a object key do arquivo no MinIO (ex: `resumes/abc-123_curriculo.pdf`). O arquivo binário fica no MinIO, não no banco.

---

## Stack

- **Backend:** ASP.NET Core 9, EF Core, MediatR (CQRS), AutoMapper, FluentValidation, MailKit, BCrypt
- **Frontend:** React 19, Vite, Tailwind CSS
- **Banco:** PostgreSQL 15+
- **Storage:** MinIO (armazenamento de currículos em PDF)
- **Auth:** Sessão simples (email + senha com BCrypt)
- **Infra:** Docker Compose

---

## Limitações Conhecidas

- Envio de e-mails é síncrono (sem fila/background job)
- Sem rate limiting no envio de e-mails

---

## Arquitetura do Backend

O backend é dividido em 6 projetos independentes dentro de `backend/src/`:

| Projeto               | Responsabilidade                                                     |
| --------------------- | -------------------------------------------------------------------- |
| `SMCV.Api`            | Host HTTP — controllers, middleware, pipeline, Program.cs            |
| `SMCV.Application`    | Contratos (interfaces), DTOs e mappings — sem implementação concreta |
| `SMCV.Domain`         | Entidades de banco e enums — zero dependência externa                |
| `SMCV.Infrastructure` | Implementações concretas — DbContext, repositories, SMTP, MinIO, CSV |
| `SMCV.Features`       | Handlers CQRS (MediatR), validators (FluentValidation)               |
| `SMCV.Common`         | Utilitários transversais — exceções, middleware                      |

O fluxo de uma requisição segue: `Controller → MediatR Handler → Repository → PostgreSQL`.  
Armazenamento de arquivos: `Controller → IFileStorageService → MinIO`.

---

## Comandos úteis

```bash
# Subir o stack completo
docker-compose up -d --build

# Parar
docker-compose down

# Ver logs do backend
docker-compose logs -f backend

# Rodar build manual (requer .NET 9 SDK)
dotnet build backend/SMCV.slnx

# Gerar nova migration
dotnet ef migrations add NomeDaMigration \
  --project backend/src/SMCV.Infrastructure \
  --startup-project backend/src/SMCV.Api
```

---

## Colaboradores

| Nome                        | Responsabilidade                      |
| --------------------------- | ------------------------------------- |
| Arthur Sampaio (aluno)      | Backend, arquitetura e banco de dados |
| Matheus Cataneu (professor) | Backend e arquitetura                 |
| Davi (aluno)                | Frontend                              |

---

## Uso de Agentes de IA

Este projeto contou com o auxílio de agentes de inteligência artificial como suporte ao desenvolvimento. As ferramentas utilizadas foram:

- **Claude (Anthropic)** — utilizado como arquiteto e tech lead assistido por IA, apoiando decisões de arquitetura, modelagem de domínio, definição de padrões de código e geração de prompts estruturados para agentes de codificação
- **Claude Code** — agente de codificação utilizado para aplicar refatorações em arquivos do projeto a partir de prompts gerados pelo Claude, executando alterações cirúrgicas e rastreáveis por camada
