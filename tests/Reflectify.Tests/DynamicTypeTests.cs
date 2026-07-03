using System.Reflection;
using Xunit;
using Reflectify;
using Reflectify.Models;

namespace Reflectify.Tests;

public class DynamicTypeTests
{
    [Fact]
    public void CreateDynamicType_SimpleType_CreatesType()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var properties = new List<DynamicPropertyInfo>
        {
            new("Name", typeof(string)),
            new("Age", typeof(int))
        };

        var dynamicType = reflectify.CreateDynamicType(
            "TestAssembly",
            "Person",
            [],
            properties);

        Assert.NotNull(dynamicType);
        Assert.Equal("Person", dynamicType.Name);

        var instance = Activator.CreateInstance(dynamicType)!;
        var nameProp = dynamicType.GetProperty("Name");
        var ageProp = dynamicType.GetProperty("Age");

        Assert.NotNull(nameProp);
        Assert.NotNull(ageProp);

        nameProp!.SetValue(instance, "John");
        ageProp!.SetValue(instance, 30);

        Assert.Equal("John", nameProp.GetValue(instance));
        Assert.Equal(30, ageProp.GetValue(instance));
    }

    [Fact]
    public void CreateDynamicType_WithBaseType_CreatesInheritedType()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var properties = new List<DynamicPropertyInfo>
        {
            new("ExtraField", typeof(string))
        };

        var dynamicType = reflectify.CreateDynamicType(
            "TestAssembly",
            "Derived",
            [],
            properties,
            typeof(BaseModel));

        Assert.NotNull(dynamicType);
        Assert.True(dynamicType.IsSubclassOf(typeof(BaseModel)));

        var instance = Activator.CreateInstance(dynamicType)!;
        var extraProp = dynamicType.GetProperty("ExtraField");
        Assert.NotNull(extraProp);
    }

    [Fact]
    public void CreateDynamicType_WithClassAttributes_AppliesAttributes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var classAttrs = new List<DynamicAttributeInfo>
        {
            new(typeof(ObsoleteAttribute), "Use NewType instead")
        };
        var properties = new List<DynamicPropertyInfo>
        {
            new("Value", typeof(int))
        };

        var dynamicType = reflectify.CreateDynamicType(
            "TestAssembly",
            "ObsoleteType",
            classAttrs,
            properties);

        var obsoleteAttr = dynamicType.GetCustomAttribute<ObsoleteAttribute>();
        Assert.NotNull(obsoleteAttr);
        Assert.Equal("Use NewType instead", obsoleteAttr!.Message);
    }

    [Fact]
    public void CreateDynamicType_WithPropertyAttributes_AppliesAttributes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var propAttrs = new List<DynamicAttributeInfo>
        {
            new(typeof(ObsoleteAttribute), "This property is obsolete")
        };
        var properties = new List<DynamicPropertyInfo>
        {
            new("OldField", typeof(string), propAttrs)
        };

        var dynamicType = reflectify.CreateDynamicType(
            "TestAssembly",
            "AttributedProps",
            [],
            properties);

        var instance = Activator.CreateInstance(dynamicType)!;
        var prop = dynamicType.GetProperty("OldField")!;
        var obsoleteAttr = prop.GetCustomAttribute<ObsoleteAttribute>();

        Assert.NotNull(obsoleteAttr);
        Assert.Equal("This property is obsolete", obsoleteAttr!.Message);
    }

    [Fact]
    public void CreateDynamicType_WithMultiplePropertiesOfDifferentTypes()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var properties = new List<DynamicPropertyInfo>
        {
            new("Id", typeof(int)),
            new("Name", typeof(string)),
            new("IsActive", typeof(bool)),
            new("CreatedAt", typeof(DateTime))
        };

        var dynamicType = reflectify.CreateDynamicType(
            "TestAssembly",
            "FullModel",
            [],
            properties);

        Assert.Equal(4, dynamicType.GetProperties().Length);
        Assert.Contains(dynamicType.GetProperties(), p => p.Name == "Id" && p.PropertyType == typeof(int));
        Assert.Contains(dynamicType.GetProperties(), p => p.Name == "Name" && p.PropertyType == typeof(string));
        Assert.Contains(dynamicType.GetProperties(), p => p.Name == "IsActive" && p.PropertyType == typeof(bool));
        Assert.Contains(dynamicType.GetProperties(), p => p.Name == "CreatedAt" && p.PropertyType == typeof(DateTime));
    }

    [Fact]
    public void CreateDynamicType_NoProperties_CreatesEmptyType()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());

        var dynamicType = reflectify.CreateDynamicType(
            "TestAssembly",
            "Empty",
            [],
            []);

        Assert.NotNull(dynamicType);
        Assert.Equal("Empty", dynamicType.Name);
        Assert.Empty(dynamicType.GetProperties());
    }

    [Fact]
    public void CreateDynamicType_InvalidAttributeConstructor_Throws()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var classAttrs = new List<DynamicAttributeInfo>
        {
            new(typeof(ParamCtorOnlyAttribute))
        };

        Assert.Throws<InvalidOperationException>(() =>
            reflectify.CreateDynamicType("TestAssembly", "BadAttr", classAttrs, []));
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ParamCtorOnlyAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

public class BaseModel
{
    public int Id { get; set; }
}
