using Xunit;
using Reflectify;
using Reflectify.Models;

namespace Reflectify.Tests;

[SampleClass("ClassSample")]
public class PropertiesTests
{
    [DisplayName("Product Name")]
    public string Name { get; set; } = "Default";

    [Range(1, 100)]
    public int Quantity { get; set; } = 10;

    public string Unattributed { get; set; } = "none";

    [Fact]
    public void GetProperties_ReturnsAllPropertiesWithAttributes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var properties = reflectify.GetProperties(typeof(PropertiesTests));

        Assert.NotNull(properties);
        Assert.Equal(3, properties.Count);
    }

    [Fact]
    public void GetProperties_PropertiesHaveCorrectAttributes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var properties = reflectify.GetProperties(typeof(PropertiesTests));

        var nameProp = properties!.First(p => p.PropertyInfo.Name == "Name");
        var displayAttr = nameProp.GetAttribute<DisplayNameAttribute>();

        Assert.NotNull(displayAttr);
        Assert.Equal("Product Name", displayAttr!.DisplayName);
    }

    [Fact]
    public void GetProperties_GenericOverload_ReturnsProperties()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new PropertiesTests();
        var properties = reflectify.GetProperties(instance);

        Assert.NotNull(properties);
        Assert.Equal(3, properties.Count);
    }

    [Fact]
    public void GetProperty_ByName_ReturnsCorrectProperty()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var property = reflectify.GetProperty(typeof(PropertiesTests), "Quantity");

        Assert.NotNull(property);
        Assert.Equal("Quantity", property!.PropertyInfo.Name);
        Assert.Equal(typeof(int), property.PropertyInfo.PropertyType);
    }

    [Fact]
    public void GetProperty_ByName_HasAttributes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var property = reflectify.GetProperty(typeof(PropertiesTests), "Quantity");

        var rangeAttr = property!.GetAttribute<RangeAttribute>();
        Assert.NotNull(rangeAttr);
        Assert.Equal(1, rangeAttr!.Min);
        Assert.Equal(100, rangeAttr.Max);
    }

    [Fact]
    public void GetProperty_NonExistent_ReturnsNull()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var property = reflectify.GetProperty(typeof(PropertiesTests), "NonExistent");

        Assert.Null(property);
    }

    [Fact]
    public void GetProperty_GenericOverload_ReturnsProperty()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var instance = new PropertiesTests();
        var property = reflectify.GetProperty(instance, "Name");

        Assert.NotNull(property);
        Assert.Equal("Name", property!.PropertyInfo.Name);
    }

    [Fact]
    public void GetProperties_UnattributedProperty_HasNoAttributes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var property = reflectify.GetProperty(typeof(PropertiesTests), "Unattributed");

        Assert.NotNull(property);
        Assert.Null(property!.GetAttribute<DisplayNameAttribute>());
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class DisplayNameAttribute(string displayName) : Attribute
{
    public string DisplayName { get; } = displayName;
}

[AttributeUsage(AttributeTargets.Property)]
public class RangeAttribute(int min, int max) : Attribute
{
    public int Min { get; } = min;
    public int Max { get; } = max;
}
