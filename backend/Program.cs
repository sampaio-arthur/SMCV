using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SMCV.Application.Interfaces;
using SMCV.Common.Middleware;
using SMCV.Infrastructure.Data;
using SMCV.Infrastructure.ExternalServices;
using SMCV.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

string RequiredSetting(string key)
{
    return Environment.GetEnvironmentVariable(key)
        ?? throw new InvalidOperationException($"Environment variable '{key}' was not configured.");
}

// ─── HttpContextAccessor ──────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();

// ─── Controllers ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ─── Swagger / OpenAPI ────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Job Prospector API",
        Version = "v1",
        Description = "API para prospecção de empregos via e-mail automatizado"
    });
});

// ─── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ─── Sessão ──────────────────────────────────────────────────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
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

// ─── Repositórios ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IEmailLogRepository, EmailLogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

// ─── Serviços Externos ───────────────────────────────────────────────────────
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<ICsvExportService, CsvExportService>();

// ─── HttpClient para Apollo.io ───────────────────────────────────────────────
builder.Services.AddHttpClient<IHunterService, HunterService>(client =>
{
    client.BaseAddress = new Uri("https://api.apollo.io/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// ─── EmailSettings via IOptions ──────────────────────────────────────────────
builder.Services.Configure<EmailSettings>(opts =>
{
    opts.SmtpHost = RequiredSetting("SMTP_HOST");
    opts.SmtpPort = int.Parse(RequiredSetting("SMTP_PORT"));
    opts.SenderEmail = RequiredSetting("SMTP_SENDER_EMAIL");
    opts.SenderPassword = RequiredSetting("SMTP_SENDER_PASSWORD");
    opts.SenderName = RequiredSetting("SMTP_SENDER_NAME");
});

// ─── Build & Pipeline ─────────────────────────────────────────────────────────
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseCors("AllowReactApp");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSession();
app.MapControllers();
app.Run();
