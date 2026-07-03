using Xunit;
using Reflectify;
using Reflectify.Models;

namespace Reflectify.Tests;

[SampleClass("ClassSample")]
public class AttributeLookupTests
{
    [SampleProperty("PropSample")]
    public string MyProperty { get; set; } = "test";

    [Fact]
    public void GetAttribute_OnType_ReturnsAttribute()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var typeInfo = reflectify.GetTypeInfo(typeof(AttributeLookupTests));

        var attr = typeInfo.GetAttribute<SampleClassAttribute>();

        Assert.NotNull(attr);
        Assert.Equal("ClassSample", attr!.Value);
    }

    [Fact]
    public void GetAttribute_OnType_Missing_ReturnsNull()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var typeInfo = reflectify.GetTypeInfo(typeof(AttributeLookupTests));

        var attr = typeInfo.GetAttribute<SampleMethodAttribute>();

        Assert.Null(attr);
    }

    [Fact]
    public void GetAttribute_OnProperty_ReturnsAttribute()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var prop = reflectify.GetProperty(typeof(AttributeLookupTests), "MyProperty");

        var attr = prop!.GetAttribute<SamplePropertyAttribute>();

        Assert.NotNull(attr);
        Assert.Equal("PropSample", attr!.Value);
    }

    [Fact]
    public void GetAttribute_OnProperty_Missing_ReturnsNull()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var prop = reflectify.GetProperty(typeof(AttributeLookupTests), "MyProperty");

        var attr = prop!.GetAttribute<SampleClassAttribute>();

        Assert.Null(attr);
    }

    [Fact]
    public void GetAttribute_OnMethod_ReturnsAttribute()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var method = reflectify.GetMethod(typeof(MethodAttribModel), "MyMethod");

        var attr = method!.GetAttribute<SampleMethodAttribute>();

        Assert.NotNull(attr);
        Assert.Equal("MethodSample", attr!.Value);
    }

    [Fact]
    public void GetAttribute_OnMethod_Missing_ReturnsNull()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var method = reflectify.GetMethod(typeof(MethodAttribModel), "MyMethod");

        var attr = method!.GetAttribute<SamplePropertyAttribute>();

        Assert.Null(attr);
    }

    [Fact]
    public void GetAttribute_OnProperty_AccessMultipleAttributes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var prop = reflectify.GetProperty(typeof(MultiAttribModel), "Name");

        Assert.NotNull(prop!.GetAttribute<SamplePropertyAttribute>());
        Assert.NotNull(prop.GetAttribute<RequiredAttribute>());
    }

    [Fact]
    public void GetAttribute_OnMethod_AccessMultipleAttributes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var method = reflectify.GetMethod(typeof(MultiAttribModel), "Save");

        Assert.NotNull(method!.GetAttribute<SampleMethodAttribute>());
        Assert.NotNull(method.GetAttribute<ObsoleteAttribute>());
    }

    [Fact]
    public void GetAttribute_UnattributedMember_ReturnsNull()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var prop = reflectify.GetProperty(typeof(MultiAttribModel), "NoAttr");

        Assert.Null(prop!.GetAttribute<SamplePropertyAttribute>());
    }

    [Fact]
    public void AttributesInfo_AttributesList_IsPopulated()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var typeInfo = reflectify.GetTypeInfo(typeof(AttributeLookupTests));

        Assert.NotNull(typeInfo.Attributes);
        Assert.NotEmpty(typeInfo.Attributes);
    }
}

public class MethodAttribModel
{
    [SampleMethod("MethodSample")]
    public void MyMethod() { }
}

[AttributeUsage(AttributeTargets.Class)]
public class SampleClassAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

[AttributeUsage(AttributeTargets.Property)]
public class SamplePropertyAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

[AttributeUsage(AttributeTargets.Method)]
public class SampleMethodAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

public class RequiredAttribute : Attribute;

public class MultiAttribModel
{
    [SampleProperty("NameField")]
    [Required]
    public string Name { get; set; } = "";

    [SampleMethod("SaveMethod")]
    [Obsolete]
    public void Save() { }

    public int NoAttr { get; set; }
}
