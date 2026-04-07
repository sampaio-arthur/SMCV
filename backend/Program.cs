using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SMCV.Application.Interfaces;
using SMCV.Infrastructure.Data;
using SMCV.Infrastructure.Repositories;
using SMCV.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

string RequiredSetting(string key)
{
    return Environment.GetEnvironmentVariable(key)
        ?? throw new InvalidOperationException($"Environment variable '{key}' was not configured.");
}

// ─── Controllers ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ─── Swagger / OpenAPI ────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ─── CORS ─────────────────────────────────────────────────────────────────────
// Permissive policy for development. Restrict in production.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ─── Database ─────────────────────────────────────────────────────────────────
var connectionString = string.Join(";", [
    $"Host={RequiredSetting("DB_HOST")}",
    $"Port={RequiredSetting("DB_PORT")}",
    $"Database={RequiredSetting("DB_NAME")}",
    $"Username={RequiredSetting("DB_USER")}",
    $"Password={RequiredSetting("DB_PASSWORD")}"
]);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ─── MediatR + FluentValidation + AutoMapper ─────────────────────────────────
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// ─── Dependency Injection ─────────────────────────────────────────────────────
builder.Services.AddScoped<IExampleRepository, ExampleRepository>();
builder.Services.AddScoped<IExampleService, ExampleService>();

// ─── Build & Pipeline ─────────────────────────────────────────────────────────
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.MapControllers();
app.Run();
