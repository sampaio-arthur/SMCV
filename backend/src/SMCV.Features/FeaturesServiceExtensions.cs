using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace SMCV.Features;

public static class FeaturesServiceExtensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        var assembly = typeof(FeaturesServiceExtensions).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
