using Xunit;
using Reflectify;
using Reflectify.Models;

namespace Reflectify.Tests;

[MyCustom("ClassLevel")]
public class TypeInfoTests
{
    [Fact]
    public void GetTypeInfo_ReturnsExtendedTypeInfo()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var typeInfo = reflectify.GetTypeInfo(typeof(TypeInfoTests));

        Assert.NotNull(typeInfo);
        Assert.Equal(typeof(TypeInfoTests), typeInfo.Type);
    }

    [Fact]
    public void GetTypeInfo_IncludesClassAttributes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var typeInfo = reflectify.GetTypeInfo(typeof(TypeInfoTests));

        var attr = typeInfo.GetAttribute<MyCustomAttribute>();
        Assert.NotNull(attr);
        Assert.Equal("ClassLevel", attr!.Value);
    }

    [Fact]
    public void GetTypeInfo_NoAttributes_ReturnsEmptyList()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var typeInfo = reflectify.GetTypeInfo(typeof(PlainClass));

        Assert.NotNull(typeInfo);
        Assert.Equal(typeof(PlainClass), typeInfo.Type);
        Assert.Null(typeInfo.GetAttribute<MyCustomAttribute>());
    }

    [Fact]
    public void GetTypeInfo_GenericOverload_ReturnsTypeInfo()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new TypeInfoTests();
        var typeInfo = reflectify.GetTypeInfo(instance);

        Assert.NotNull(typeInfo);
        Assert.Equal(typeof(TypeInfoTests), typeInfo!.Type);
    }

    [Fact]
    public void GetTypeInfo_GenericOverload_IncludesClassAttributes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new TypeInfoTests();
        var typeInfo = reflectify.GetTypeInfo(instance);

        var attr = typeInfo!.GetAttribute<MyCustomAttribute>();
        Assert.NotNull(attr);
        Assert.Equal("ClassLevel", attr!.Value);
    }

    [Fact]
    public void GetTypeInfo_CanRetrieveMultipleAttributes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var typeInfo = reflectify.GetTypeInfo(typeof(MultiAttributedClass));

        var attr1 = typeInfo.GetAttribute<MyCustomAttribute>();
        var attr2 = typeInfo.GetAttribute<AnotherAttribute>();

        Assert.NotNull(attr1);
        Assert.NotNull(attr2);
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class MyCustomAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

[AttributeUsage(AttributeTargets.Class)]
public class AnotherAttribute : Attribute;

[MyCustom("First")]
[Another]
public class MultiAttributedClass;

public class PlainClass;
