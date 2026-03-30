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
4. AddDbContext<AppDbContext>()
5. AddScoped — Repositories (IXxxRepository, XxxRepository)
6. AddScoped — Services (IXxxService, XxxService)
```

## COMO REGISTRAR NOVO RECURSO

```csharp
// Em "Dependency Injection", apos os registros existentes:
builder.Services.AddScoped<INovoRepository, NovoRepository>();
builder.Services.AddScoped<INovoService, NovoService>();
```

- Sempre usar `AddScoped` (um por request HTTP)
- Sempre registrar Repository ANTES do Service correspondente
- Adicionar os `using` necessarios no topo do arquivo

## CORS

Configuracao atual: politica `"AllowAll"` (permissiva para desenvolvimento).
```csharp
policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
```
Para producao: restringir origins, methods e headers.
Localizacao: bloco `AddCors(options => ...)` antes do `AddDbContext`.

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

Habilitado apenas em Development:
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

## PIPELINE HTTP (ordem importa)

```
app.UseCors("AllowAll");
app.MapControllers();
app.Run();
```

Para adicionar middleware: inserir ENTRE `UseCors` e `MapControllers`.
Exemplo: `app.UseAuthentication()` e `app.UseAuthorization()`.

## CONNECTION STRING

Construida a partir de variaveis de ambiente individuais:
`DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASSWORD`.
Helper `RequiredSetting()` lanca excecao se alguma estiver ausente.
