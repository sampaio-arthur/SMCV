using Microsoft.Extensions.DependencyInjection;

namespace SMCV.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationServiceExtensions).Assembly);

        return services;
    }
}
