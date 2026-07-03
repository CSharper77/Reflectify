using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Reflectify;
using Reflectify.Models;
using Reflectify.Extensions;

namespace Reflectify.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddReflectify_RegistersReflectifyAsSingleton()
    {
        var services = new ServiceCollection();

        services.AddReflectify(_ => { });

        var provider = services.BuildServiceProvider();
        var instance = provider.GetService<IReflectify>();

        Assert.NotNull(instance);
        Assert.IsType<Reflectify>(instance);
    }

    [Fact]
    public void AddReflectify_Singleton_SameInstance()
    {
        var services = new ServiceCollection();

        services.AddReflectify(_ => { });

        var provider = services.BuildServiceProvider();
        var first = provider.GetService<IReflectify>();
        var second = provider.GetService<IReflectify>();

        Assert.Same(first, second);
    }

    [Fact]
    public void AddReflectify_Scoped_DifferentScopeDifferentInstance()
    {
        var services = new ServiceCollection();

        services.AddReflectify(config => config.LifeTime = LifeTime.Scoped);

        var provider = services.BuildServiceProvider();
        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();
        var first = scope1.ServiceProvider.GetService<IReflectify>();
        var second = scope2.ServiceProvider.GetService<IReflectify>();

        Assert.NotSame(first, second);
    }

    [Fact]
    public void AddReflectify_Transient_DifferentInstance()
    {
        var services = new ServiceCollection();

        services.AddReflectify(config => config.LifeTime = LifeTime.Transient);

        var provider = services.BuildServiceProvider();
        var first = provider.GetService<IReflectify>();
        var second = provider.GetService<IReflectify>();

        Assert.NotSame(first, second);
    }

    [Fact]
    public void AddReflectify_ManualRegistration_ResolvesIReflectify()
    {
        var services = new ServiceCollection();

        services.AddSingleton(new ReflectifyConfiguration());
        services.AddSingleton<IReflectify, Reflectify>();

        var provider = services.BuildServiceProvider();
        var instance = provider.GetService<IReflectify>();

        Assert.NotNull(instance);
        Assert.IsType<Reflectify>(instance);
    }

    [Fact]
    public void AddReflectify_ServiceCanBeUsedAfterResolution()
    {
        var services = new ServiceCollection();

        services.AddReflectify(_ => { });

        var provider = services.BuildServiceProvider();
        var reflectify = provider.GetService<IReflectify>();

        var properties = reflectify!.GetProperties(typeof(DITestModel));

        Assert.NotNull(properties);
        Assert.Single(properties);
    }

    [Fact]
    public void AddReflectify_WorksWithDIContainerInConsumer()
    {
        var services = new ServiceCollection();

        services.AddReflectify(_ => { });
        services.AddTransient<ReflectifyConsumer>();

        var provider = services.BuildServiceProvider();
        var consumer = provider.GetService<ReflectifyConsumer>();

        Assert.NotNull(consumer);
        var typeInfo = consumer!.Analyze<DITestModel>();

        Assert.NotNull(typeInfo);
        Assert.Equal(typeof(DITestModel), typeInfo.Type);
    }

    [Fact]
    public void AddReflectify_DefaultLifeTime_IsSingleton()
    {
        var services = new ServiceCollection();

        services.AddReflectify(_ => { });

        var config = services.BuildServiceProvider().GetService<ReflectifyConfiguration>();

        Assert.NotNull(config);
        Assert.Equal(LifeTime.Singleton, config!.LifeTime);
        Assert.Equal(DetectionVisibility.OnlyPublic, config.DetectionVisibility);
    }
}

public class ReflectifyConsumer
{
    private readonly IReflectify _reflectify;

    public ReflectifyConsumer(IReflectify reflectify)
    {
        _reflectify = reflectify;
    }

    public ExtendedTypeInfo Analyze<T>() where T : class
    {
        return _reflectify.GetTypeInfo(typeof(T));
    }
}

public class DITestModel
{
    public string Name { get; set; } = "";
}
