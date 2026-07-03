using Xunit;
using Reflectify;
using Reflectify.Models;

namespace Reflectify.Tests;

public class MethodsTests
{
    [Fact]
    public void GetMethods_ReturnsAllPublicMethods()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var methods = reflectify.GetMethods(typeof(MethodsTestModel));

        Assert.NotNull(methods);
        Assert.Contains(methods, m => m.Method.Name == "Execute");
        Assert.Contains(methods, m => m.Method.Name == "Process");
    }

    [Fact]
    public void GetMethods_IncludesInheritedMethods()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var methods = reflectify.GetMethods(typeof(MethodsTestModel));

        Assert.Contains(methods, m => m.Method.Name == "ToString");
        Assert.Contains(methods, m => m.Method.Name == "GetHashCode");
    }

    [Fact]
    public void GetMethods_GenericOverload_ReturnsMethods()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new MethodsTestModel();
        var methods = reflectify.GetMethods(instance);

        Assert.NotNull(methods);
        Assert.Contains(methods, m => m.Method.Name == "Execute");
    }

    [Fact]
    public void GetMethod_ByName_ReturnsCorrectMethod()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var method = reflectify.GetMethod(typeof(MethodsTestModel), "Execute");

        Assert.NotNull(method);
        Assert.Equal("Execute", method!.Method.Name);
    }

    [Fact]
    public void GetMethod_NonExistent_ReturnsNull()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var method = reflectify.GetMethod(typeof(MethodsTestModel), "NonExistentMethod");

        Assert.Null(method);
    }

    [Fact]
    public void GetMethod_GenericOverload_ReturnsMethod()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new MethodsTestModel();
        var method = reflectify.GetMethod(instance, "Process");

        Assert.NotNull(method);
        Assert.Equal("Process", method!.Method.Name);
    }

    [Fact]
    public void GetMethod_CaseSensitive()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var methodLower = reflectify.GetMethod(typeof(MethodsTestModel), "execute");
        var methodUpper = reflectify.GetMethod(typeof(MethodsTestModel), "EXECUTE");

        Assert.Null(methodLower);
        Assert.Null(methodUpper);
    }
}

public class MethodsTestModel
{
    public string Execute() => "executed";
    public int Process(int x, int y) => x + y;
}
