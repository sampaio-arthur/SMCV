using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ─── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ─── Autenticação Keycloak (JWT Bearer) ───────────────────────────────────────
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = RequiredSetting("KEYCLOAK_AUTHORITY");
    options.Audience = RequiredSetting("KEYCLOAK_AUDIENCE");
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
    };
});

builder.Services.AddAuthorization();

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
builder.Services.AddScoped<IHunterService, HunterService>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<ICsvExportService, CsvExportService>();

// ─── HttpClient para Hunter.io ───────────────────────────────────────────────
builder.Services.AddHttpClient<HunterService>();

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
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();
app.Run();
