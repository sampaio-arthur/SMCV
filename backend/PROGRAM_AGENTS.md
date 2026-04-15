# Program.cs — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.
> Arquivo: `src/SMCV.Api/Program.cs`

## RESPONSABILIDADE

Configuracao do host, registro de dependencias (DI) e pipeline HTTP.
Arquivo unico, sem classe — usa top-level statements.

## ORDEM DE REGISTRO NO DI

Manter esta ordem no `Program.cs`:
```
0. AddHttpContextAccessor()
1. AddControllers()
2. AddEndpointsApiExplorer() + AddSwaggerGen()
3. AddCors()
4. AddDistributedMemoryCache() + AddSession()
5. AddApplication()       — AutoMapper (SMCV.Application)
6. AddFeatures()          — MediatR + FluentValidation (SMCV.Features)
7. AddInfrastructure()    — DbContext, Repositories, External Services, IOptions (SMCV.Infrastructure)
```

## STARTUP MODULES

Cada camada registra seus proprios servicos via extension method em `IServiceCollection`.
O `Program.cs` NAO conhece implementacoes concretas de repositories ou services — apenas chama os modulos.

| Modulo | Arquivo | Registra |
|--------|---------|----------|
| `AddApplication()` | `src/SMCV.Application/ApplicationServiceExtensions.cs` | AutoMapper (assembly scan) |
| `AddFeatures()` | `src/SMCV.Features/FeaturesServiceExtensions.cs` | MediatR + FluentValidation (assembly scan) |
| `AddInfrastructure(connectionString, configureEmail, configureMinio)` | `src/SMCV.Infrastructure/InfrastructureServiceExtensions.cs` | DbContext, Repositories, External Services, EmailSettings via IOptions, MinIO (IMinioClient Singleton, MinioSettings Singleton, IFileStorageService Scoped) |

## COMO REGISTRAR NOVO RECURSO

- **Novo Repository:** adicionar `AddScoped<IXxx, Xxx>()` em `InfrastructureServiceExtensions.cs`
- **Novo External Service:** adicionar `AddScoped<IXxx, Xxx>()` em `InfrastructureServiceExtensions.cs`
- **Novo Handler/Validator:** automatico via assembly scan em `AddFeatures()`
- **Novo Mapping Profile:** automatico via assembly scan em `AddApplication()`

NAO registrar manualmente no `Program.cs` — usar o extension method da camada correspondente.

## CORS

Configuracao atual: politica `"AllowReactApp"` com origins especificos e credentials habilitados para sessao.
```csharp
policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();
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
app.UseSession();
app.MapControllers();
app.Run();
```

`ExceptionHandlingMiddleware` fica ANTES de `UseSession` para capturar erros.
`UseSession()` deve estar ANTES de `MapControllers()`.

## CONNECTION STRING

Construida a partir de variaveis de ambiente individuais:
`DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASSWORD`.
Helper `RequiredSetting()` lanca excecao se alguma estiver ausente.
