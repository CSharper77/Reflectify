using Microsoft.Extensions.DependencyInjection;
using Reflectify.Models;

namespace Reflectify.Extensions;

/// <summary>Extension methods for registering Reflectify services.</summary>
public static class Extensions
{
    /// <summary>Registers the <see cref="Reflectify"/> implementation as a singleton.</summary>
    public static void AddReflectify(this IServiceCollection services, Action<ReflectifyConfiguration> options)
    {
        var config = new ReflectifyConfiguration();
        options?.Invoke(config);

        services.AddSingleton(config);

        switch (config.LifeTime)
        {
            case LifeTime.Singleton:
                services.AddSingleton<IReflectify, Reflectify>();
                break;
            case LifeTime.Scoped:
                services.AddScoped<IReflectify, Reflectify>();
                break;
            case LifeTime.Transient:
                services.AddTransient<IReflectify, Reflectify>();
                break;

        }
    }
}