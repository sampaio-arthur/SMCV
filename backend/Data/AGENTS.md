# Data — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

`AppDbContext` e o ponto central do EF Core. Registra as Entities como `DbSet<T>`
e gerencia a conexao com PostgreSQL.

## REGRAS OBRIGATORIAS

- Para cada nova Entity, adicionar `public DbSet<Xxx> Xxxs { get; set; }` no `AppDbContext`
- Nomenclatura do DbSet: **plural em ingles** (ex: `Examples`, `Products`, `Categories`)
- Herdar de `DbContext`, receber `DbContextOptions<AppDbContext>` no construtor
- Relacionamentos complexos configurar via `OnModelCreating` com Fluent API (quando necessario)

## MIGRATIONS

Criar migration:
```bash
cd backend
dotnet ef migrations add NomeDaMigration
```

Aplicar migration:
```bash
dotnet ef database update
```

**Regras:**
- Nome da migration em PascalCase descritivo (ex: `AddProductTable`, `AddCategoryIdToProduct`)
- **NUNCA** deletar migrations ja aplicadas em producao
- **NUNCA** editar manualmente arquivos em `Migrations/` ja aplicados
- Ao adicionar nova Entity: criar migration imediatamente apos adicionar o `DbSet`

## AUTO-MIGRATE

O `Program.cs` executa `dbContext.Database.Migrate()` no startup.
Isso aplica migrations pendentes automaticamente ao iniciar a aplicacao.
NAO mover nem remover esse trecho.

## FLUENT API (quando usar)

Usar `OnModelCreating` para:
- Configurar chaves compostas
- Definir indices unicos
- Customizar nomes de tabela/coluna
- Configurar cascade delete diferente do padrao

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Xxx>()
        .HasIndex(x => x.Email)
        .IsUnique();
}
```

## REFERENCIA RAPIDA

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Example> Examples { get; set; }
    // Adicionar novos DbSets aqui
}
```
