# SMCV — Job Prospector

Sistema de prospecção automatizada de empregos via e-mail. Permite gerenciamento de campanhas, contatos e disparo de e-mails com currículo anexado.

## Problema que resolve

Automatiza o processo de organizar campanhas de e-mail e enviar currículos em massa — reduzindo o tempo de prospecção de horas para minutos.

## Pré-requisitos

- Docker e Docker Compose
- Conta Gmail (para envio de e-mails via SMTP)

## Configuração

### 1. Criar o arquivo `.env`

Copie o arquivo de exemplo:

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

O sistema usa SMTP para enviar e-mails. Para usar o Gmail, você precisa gerar uma **Senha de App** (App Password):

1. Acesse sua conta Google em [myaccount.google.com](https://myaccount.google.com)
2. Vá em **Segurança**
3. Na seção **Como fazer login no Google**, ative a **Verificação em duas etapas** (obrigatório)
4. Depois de ativar, volte em **Segurança** → **Verificação em duas etapas** → **Senhas de app** (no final da página)
5. Crie uma nova senha de app selecionando **Outro** e dando um nome (ex: "Job Prospector")
6. O Google vai gerar uma senha de 16 caracteres (ex: `abcd efgh ijkl mnop`)
7. Copie essa senha e cole no `.env` **sem espaços**

```env
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_SENDER_EMAIL=seu_email@gmail.com
SMTP_SENDER_PASSWORD=abcdefghijklmnop
SMTP_SENDER_NAME=Job Prospector
```

> **Nunca use sua senha pessoal do Google.** Use sempre a Senha de App gerada.
> **Nunca commite o arquivo `.env`.**

## Execução

```bash
docker-compose up -d --build
```

O Docker Compose sobe os três serviços (banco, backend e frontend). O banco é criado e as migrations são aplicadas automaticamente no startup.

| Serviço     | URL                           |
| ----------- | ----------------------------- |
| Frontend    | http://localhost:3000         |
| Backend API | http://localhost:8080         |
| Swagger     | http://localhost:8080/swagger |

Para parar:

```bash
docker-compose down
```

## Endpoints

Documentação interativa disponível em `/swagger`.

Autenticação via sessão (email + senha). Endpoints de auth:

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
| POST   | `/api/campaigns`                       | Criar campanha (JSON)             |
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

- **Backend:** ASP.NET Core 9, EF Core, MediatR (CQRS), AutoMapper, FluentValidation, MailKit, BCrypt
- **Frontend:** React 19, Vite, Tailwind CSS
- **Banco:** PostgreSQL 15+
- **Auth:** Sessão simples (email + senha com BCrypt)
- **Infra:** Docker Compose

## Limitações Conhecidas

- Envio de e-mails é síncrono (sem fila/background job)
- Sem rate limiting no envio de e-mails

---

## Arquitetura do Backend — Decisões Estruturais

O objetivo é alinhar o projeto com práticas adotadas em equipes de mercado, mesmo sendo um projeto de menor porte onde parte dessa estrutura seria opcional.

### Separação em múltiplos projetos (.csproj)

O backend foi dividido em 6 projetos independentes dentro de `backend/src/`:

| Projeto               | Responsabilidade                                                           |
| --------------------- | -------------------------------------------------------------------------- |
| `SMCV.Api`            | Host HTTP — controllers, middleware, pipeline, Program.cs                  |
| `SMCV.Application`    | Contratos (interfaces), DTOs e mappings — sem implementação concreta       |
| `SMCV.Domain`         | Entidades de banco e enums — zero dependência externa                      |
| `SMCV.Infrastructure` | Implementações concretas — DbContext, repositories, serviços SMTP/CSV      |
| `SMCV.Features`       | Handlers CQRS (MediatR), validators (FluentValidation) — lógica de negócio |
| `SMCV.Common`         | Utilitários transversais — Result pattern, exceções, middleware            |

Cada projeto define um **boundary de compilação**, o que significa que o compilador impede violações de arquitetura automaticamente. Por exemplo, `SMCV.Domain` não consegue referenciar `SMCV.Infrastructure` — se alguém tentar usar o `DbContext` dentro de uma entidade, o código simplesmente não compila. Em um projeto monolítico, essa restrição existiria apenas como convenção (facilmente quebrada).

Em projetos .NET de médio/grande porte, essa separação é padrão (Clean Architecture, Onion Architecture, Hexagonal). Permite que equipes diferentes trabalhem em camadas diferentes com menor risco de conflito e facilita a adoção futura de testes unitários por camada.

### Solution file (.slnx)

O `.sln` tradicional foi substituído por um `.slnx` (formato XML simplificado do .NET 9+), que agrupa os 6 projetos em um único ponto de build:

```bash
dotnet build SMCV.slnx    # compila tudo de uma vez
```

### Central Package Management (Directory.Packages.props)

Todas as versões de pacotes NuGet são definidas em um único arquivo `Directory.Packages.props` na raiz do backend. Cada `.csproj` apenas declara **qual** pacote usa, sem repetir a versão:

```xml
<!-- No .csproj: sem versão -->
<PackageReference Include="MediatR" />

<!-- No Directory.Packages.props: versão centralizada -->
<PackageVersion Include="MediatR" Version="12.4.1" />
```

Sem essa centralização, cada projeto pode acabar em versões diferentes do mesmo pacote (um com MediatR 12.0, outro com 12.4), causando bugs sutis em runtime. Em solutions com dezenas de projetos, é considerada prática padrão.

### Directory.Build.props (configuração compartilhada)

Propriedades comuns como `TargetFramework`, `Nullable` e `ImplicitUsings` são definidas uma única vez em `Directory.Build.props` e herdadas por todos os projetos. Isso evita duplicação e garante consistência.

### Startup Modules (extension methods de DI)

Cada camada registra seus próprios serviços no container de injeção de dependência, em vez de concentrar tudo no `Program.cs`:

| Módulo                               | Arquivo                              | O que registra                                            |
| ------------------------------------ | ------------------------------------ | --------------------------------------------------------- |
| `AddApplication()`                   | `ApplicationServiceExtensions.cs`    | AutoMapper                                                |
| `AddFeatures()`                      | `FeaturesServiceExtensions.cs`       | MediatR + FluentValidation                                |
| `AddInfrastructure(conn, emailOpts)` | `InfrastructureServiceExtensions.cs` | DbContext, repositories, services externos, EmailSettings |

**Antes** — o `Program.cs` conhecia cada repositório e serviço pelo nome:

```csharp
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
// ... mais 5 registros manuais
```

**Depois** — o `Program.cs` só conhece os módulos:

```csharp
builder.Services.AddApplication();
builder.Services.AddFeatures();
builder.Services.AddInfrastructure(connectionString, opts => { ... });
```

Isso completa a separação em projetos. Sem os startup modules, o `Program.cs` (que fica no `Api`) precisaria conhecer todas as classes concretas do `Infrastructure`, anulando o benefício do boundary de compilação. Com essa abordagem, cada camada é autônoma: ao criar um repositório novo, basta registrá-lo no extension method da própria camada, sem tocar no `Program.cs`.
