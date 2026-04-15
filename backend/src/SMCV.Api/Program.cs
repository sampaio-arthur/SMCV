using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SMCV.Application;
using SMCV.Common.Middleware;
using SMCV.Features;
using SMCV.Infrastructure;
using SMCV.Infrastructure.Data;
using SMCV.Infrastructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);
const long DefaultResumeMaxFileSizeBytes = 25 * 1024 * 1024;
const string ResumeMaxFileSizeConfigKey = "ResumeUpload:MaxFileSizeBytes";
var resumeMaxFileSizeBytes = builder.Configuration.GetValue<long?>(ResumeMaxFileSizeConfigKey)
    ?? DefaultResumeMaxFileSizeBytes;

string RequiredSetting(string key)
{
    return Environment.GetEnvironmentVariable(key)
        ?? throw new InvalidOperationException($"Environment variable '{key}' was not configured.");
}

static bool IsResumeUploadRequest(PathString path)
{
    return path.StartsWithSegments("/api/userprofiles", out var remainingPath)
        && remainingPath.Value?.EndsWith("/upload-resume", StringComparison.OrdinalIgnoreCase) == true;
}

// ─── HttpContextAccessor ──────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = resumeMaxFileSizeBytes;
});

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

// ─── Application / Features / Infrastructure ─────────────────────────────────
var connectionString = string.Join(";", [
    $"Host={RequiredSetting("DB_HOST")}",
    $"Port={RequiredSetting("DB_PORT")}",
    $"Database={RequiredSetting("DB_NAME")}",
    $"Username={RequiredSetting("DB_USER")}",
    $"Password={RequiredSetting("DB_PASSWORD")}"
]);

builder.Services.AddApplication();
builder.Services.AddFeatures();
builder.Services.AddInfrastructure(connectionString,
    email =>
    {
        email.SmtpHost = RequiredSetting("SMTP_HOST");
        email.SmtpPort = int.Parse(RequiredSetting("SMTP_PORT"));
        email.SenderEmail = RequiredSetting("SMTP_SENDER_EMAIL");
        email.SenderPassword = RequiredSetting("SMTP_SENDER_PASSWORD");
        email.SenderName = RequiredSetting("SMTP_SENDER_NAME");
    },
    minio =>
    {
        minio.Endpoint = RequiredSetting("MINIO_ENDPOINT");
        minio.AccessKey = RequiredSetting("MINIO_ACCESS_KEY");
        minio.SecretKey = RequiredSetting("MINIO_SECRET_KEY");
        minio.BucketName = RequiredSetting("MINIO_BUCKET_NAME");
        minio.UseSSL = bool.TryParse(Environment.GetEnvironmentVariable("MINIO_USE_SSL"), out var ssl) && ssl;
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
app.Use(async (context, next) =>
{
    if (IsResumeUploadRequest(context.Request.Path))
    {
        var maxRequestBodySizeFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
        if (maxRequestBodySizeFeature is { IsReadOnly: false })
        {
            maxRequestBodySizeFeature.MaxRequestBodySize = resumeMaxFileSizeBytes;
        }
    }

    await next();
});
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSession();
app.MapControllers();
app.Run();
