using System.Reflection;
using Xunit;
using Reflectify;
using Reflectify.Models;

namespace Reflectify.Tests;

public class MethodInvocationTests
{
    [Fact]
    public void Invoke_NoArgs_ReturnsResult()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new Calculator();
        var method = reflectify.GetMethod(typeof(Calculator), "GetValue");

        var result = method!.Invoke(instance);

        Assert.Equal(42, result);
    }

    [Fact]
    public void Invoke_WithArgs_ReturnsResult()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new Calculator();
        var method = reflectify.GetMethod(typeof(Calculator), "Add");

        var result = method!.Invoke(instance, 3, 4);

        Assert.Equal(7, result);
    }

    [Fact]
    public void Invoke_WithStringArgs_ReturnsResult()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new Calculator();
        var method = reflectify.GetMethod(typeof(Calculator), "Concat");

        var result = method!.Invoke(instance, "Hello", "World");

        Assert.Equal("HelloWorld", result);
    }

    [Fact]
    public void Invoke_StaticMethod_WorksCorrectly()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var method = reflectify.GetMethod(typeof(Calculator), "StaticIncrement");

        var result = method!.Invoke(null, 5);

        Assert.Equal(6, result);
    }

    [Fact]
    public void Invoke_GenericTyped_ReturnsCorrectType()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new Calculator();
        var method = reflectify.GetMethod(typeof(Calculator), "GetValue");

        var result = method!.Invoke<int>(instance);

        Assert.Equal(42, result);
        Assert.IsType<int>(result);
    }

    [Fact]
    public void Invoke_GenericMethod_WithTypeArguments()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new Calculator();
        var method = reflectify.GetMethod(typeof(Calculator), "CreateInstance");

        var result = method!.Invoke<Calculator>(instance, [typeof(Calculator)]);

        Assert.NotNull(result);
        Assert.IsType<Calculator>(result);
    }

    [Fact]
    public void Invoke_VoidMethod_ReturnsNull()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new Calculator();
        var method = reflectify.GetMethod(typeof(Calculator), "Reset");

        var result = method!.Invoke(instance);

        Assert.Null(result);
        Assert.True(instance.IsReset);
    }

    [Fact]
    public void Invoke_MethodWithRefParams_ThrowsException()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new Calculator();
        var method = reflectify.GetMethod(typeof(Calculator), "TryParse");

        Assert.Throws<TargetParameterCountException>(() => method!.Invoke(instance, "42"));
    }
}

public class Calculator
{
    public bool IsReset { get; private set; }

    public int GetValue() => 42;

    public int Add(int a, int b) => a + b;

    public string Concat(string a, string b) => a + b;

    public static int StaticIncrement(int x) => x + 1;

    public void Reset() => IsReset = true;

    public T CreateInstance<T>() where T : new() => new T();

    public bool TryParse(string input, out int result)
    {
        result = int.Parse(input);
        return true;
    }
}
