using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using SMCV.Application.Interfaces;
using SMCV.Infrastructure.Data;
using SMCV.Infrastructure.ExternalServices;
using SMCV.Infrastructure.Repositories;

namespace SMCV.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        Action<EmailSettings> configureEmail,
        Action<MinioSettings> configureMinio)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IEmailLogRepository, EmailLogRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

        // External Services
        services.AddScoped<IEmailSenderService, EmailSenderService>();
        services.AddScoped<ICsvExportService, CsvExportService>();

        // Email Settings
        services.Configure(configureEmail);

        // MinIO Settings
        var minioSettings = new MinioSettings();
        configureMinio(minioSettings);
        services.AddSingleton(minioSettings);

        // MinIO Client
        services.AddSingleton<IMinioClient>(sp =>
        {
            return new MinioClient()
                .WithEndpoint(minioSettings.Endpoint)
                .WithCredentials(minioSettings.AccessKey, minioSettings.SecretKey)
                .WithSSL(minioSettings.UseSSL)
                .Build();
        });

        // File Storage Service
        services.AddScoped<IFileStorageService, MinioFileStorageService>();

        return services;
    }
}
