using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        Action<EmailSettings> configureEmail)
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

        return services;
    }
}
