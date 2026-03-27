using Microsoft.EntityFrameworkCore;
using DesenvWebApi.Data;
using DesenvWebApi.Repositories;
using DesenvWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

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
// DB_HOST is injected by Docker Compose; falls back to "localhost" for local dev.
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")!
    .Replace("{DB_HOST}", dbHost);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ─── Dependency Injection ─────────────────────────────────────────────────────
builder.Services.AddScoped<IExampleRepository, ExampleRepository>();
builder.Services.AddScoped<IExampleService, ExampleService>();

// ─── Build & Pipeline ─────────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.MapControllers();
app.Run();
