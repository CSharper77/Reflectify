using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Reflectify;

/// <summary>Extension methods for registering Reflectify services.</summary>
public static class Extensions
{
    /// <summary>Registers the <see cref="Reflection"/> implementation as a singleton.</summary>
    public static void AddReflectionHelper(this IServiceCollection services)
    {
        services.AddSingleton<Reflection>();
    }
}