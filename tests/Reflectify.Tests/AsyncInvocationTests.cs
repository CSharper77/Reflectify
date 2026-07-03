using Xunit;
using Reflectify;
using Reflectify.Models;

namespace Reflectify.Tests;

public class AsyncInvocationTests
{
    [Fact]
    public async Task InvokeAsync_TaskMethod_ReturnsResult()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new AsyncService();
        var method = reflectify.GetMethod(typeof(AsyncService), "FetchDataAsync");

        var result = await method!.InvokeAsync(instance);

        Assert.NotNull(result);
        Assert.Equal("data", result);
    }

    [Fact]
    public async Task InvokeAsync_TypedResult_ReturnsCorrectType()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new AsyncService();
        var method = reflectify.GetMethod(typeof(AsyncService), "FetchDataAsync");

        var result = await method!.InvokeAsync<string>(instance);

        Assert.Equal("data", result);
    }

    [Fact]
    public async Task InvokeAsync_WithArgs_ReturnsResult()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new AsyncService();
        var method = reflectify.GetMethod(typeof(AsyncService), "MultiplyAsync");

        var result = await method!.InvokeAsync(instance, null, 6, 7);

        Assert.Equal(42, result);
    }

    [Fact]
    public async Task InvokeAsync_TypedWithArgs_ReturnsCorrectType()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new AsyncService();
        var method = reflectify.GetMethod(typeof(AsyncService), "MultiplyAsync");

        var result = await method!.InvokeAsync(instance, null, 6, 7);

        Assert.Equal(42, result);
        Assert.IsType<int>(result);
    }

    [Fact]
    public async Task InvokeVoidAsync_TaskMethod_Completes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new AsyncService();
        var method = reflectify.GetMethod(typeof(AsyncService), "ProcessAsync");

        await method!.InvokeVoidAsync(instance);

        Assert.True(instance.Processed);
    }

    [Fact]
    public async Task InvokeAsync_GenericMethodWithTypeArgs()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new AsyncService();
        var method = reflectify.GetMethod(typeof(AsyncService), "GetAsync");

        var result = await method!.InvokeAsync<string>(instance, [typeof(string)], "hello");

        Assert.Equal("hello", result);
    }

    [Fact]
    public async Task InvokeVoidAsync_GenericMethod_Completes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new AsyncService();
        var method = reflectify.GetMethod(typeof(AsyncService), "ExecuteAsync");

        await method!.InvokeVoidAsync(instance, [typeof(string)], "test");

        Assert.Equal("test", instance.LastExecuteArg);
    }

    [Fact]
    public async Task InvokeAsync_SyncMethod_ThrowsArgumentException()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new AsyncService();
        var method = reflectify.GetMethod(typeof(AsyncService), "SyncMethod");

        await Assert.ThrowsAsync<ArgumentException>(() => method!.InvokeAsync(instance));
    }
}

public class AsyncService
{
    public bool Processed { get; private set; }
    public string? LastExecuteArg { get; private set; }

    public Task<string> FetchDataAsync() => Task.FromResult("data");

    public Task<int> MultiplyAsync(int a, int b) => Task.FromResult(a * b);

    public Task ProcessAsync()
    {
        Processed = true;
        return Task.CompletedTask;
    }

    public async Task<T> GetAsync<T>(T input)
    {
        await Task.Yield();
        return input;
    }

    public async Task ExecuteAsync<T>(T arg)
    {
        await Task.Yield();
        LastExecuteArg = arg?.ToString();
    }

    public string SyncMethod() => "not async";
}
