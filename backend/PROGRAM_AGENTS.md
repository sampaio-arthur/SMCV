# Program.cs — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Configuracao do host, registro de dependencias (DI) e pipeline HTTP.
Arquivo unico, sem classe — usa top-level statements.

## ORDEM DE REGISTRO NO DI

Manter esta ordem no `Program.cs`:
```
1. AddControllers()
2. AddEndpointsApiExplorer() + AddSwaggerGen()
3. AddCors()
4. AddAuthentication (JWT Bearer — Keycloak)
5. AddDbContext<AppDbContext>()
6. AddMediatR() + AddValidatorsFromAssembly() + AddAutoMapper()
7. AddScoped — Repositories
8. AddScoped — External Services
9. AddHttpClient — clientes HTTP tipados
10. Configure<T> — configuracoes via IOptions
```

## REGISTROS ATUAIS NO DI

### Repositories
```csharp
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IEmailLogRepository, EmailLogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
```

### External Services
```csharp
builder.Services.AddScoped<IHunterService, HunterService>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<ICsvExportService, CsvExportService>();
```

### HttpClient e Options
```csharp
builder.Services.AddHttpClient<HunterService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
```

## COMO REGISTRAR NOVO RECURSO

```csharp
// Em "Repositórios", apos os registros existentes:
builder.Services.AddScoped<INovoRepository, NovoRepository>();

// Em "Serviços Externos":
builder.Services.AddScoped<INovoService, NovoService>();
```

- Sempre usar `AddScoped` (um por request HTTP)
- Adicionar os `using` necessarios no topo do arquivo

## CORS

Configuracao atual: politica `"AllowReactApp"` com origins especificos.
```csharp
policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
      .AllowAnyHeader()
      .AllowAnyMethod();
```
Para producao: restringir origins conforme necessario.

## AUTO-MIGRATE

```csharp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
```
Executa ANTES de `app.Run()`. Aplica migrations pendentes automaticamente.
**NAO mover, NAO remover, NAO condicionar a ambiente.**

## SWAGGER

Habilitado para todos os ambientes (sem condicional):
```csharp
app.UseSwagger();
app.UseSwaggerUI();
```

## PIPELINE HTTP (ordem importa)

```
app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseCors("AllowReactApp");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

Para adicionar middleware: inserir ENTRE `UseCors` e `UseAuthentication`.
`UseAuthentication()` e `UseAuthorization()` devem estar ANTES de `MapControllers()`.

## CONNECTION STRING

Construida a partir de variaveis de ambiente individuais:
`DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASSWORD`.
Helper `RequiredSetting()` lanca excecao se alguma estiver ausente.
