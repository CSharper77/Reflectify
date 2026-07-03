using System.Collections;
using Xunit;
using Reflectify;
using Reflectify.Models;

namespace Reflectify.Tests;

public class TypeUtilitiesTests
{
    [Fact]
    public void IsCustomClassType_RegularClass_ReturnsTrue()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        Assert.True(reflectify.IsCustomClassType(typeof(MyDto)));
    }

    [Fact]
    public void IsCustomClassType_String_ReturnsFalse()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        Assert.False(reflectify.IsCustomClassType(typeof(string)));
    }

    [Fact]
    public void IsCustomClassType_ValueType_ReturnsFalse()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        Assert.False(reflectify.IsCustomClassType(typeof(int)));
        Assert.False(reflectify.IsCustomClassType(typeof(DateTime)));
        Assert.False(reflectify.IsCustomClassType(typeof(Guid)));
    }

    [Fact]
    public void IsCustomClassType_Interface_ReturnsFalse()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        Assert.False(reflectify.IsCustomClassType(typeof(IDisposable)));
    }

    [Fact]
    public void IsCustomClassType_AbstractClass_ReturnsTrue()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        Assert.True(reflectify.IsCustomClassType(typeof(AbstractBase)));
    }

    [Fact]
    public void IsCollectionType_List_ReturnsTrue()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        Assert.True(reflectify.IsCollectionType(typeof(List<int>)));
        Assert.True(reflectify.IsCollectionType(typeof(List<string>)));
    }

    [Fact]
    public void IsCollectionType_Array_ReturnsTrue()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        Assert.True(reflectify.IsCollectionType(typeof(int[])));
        Assert.True(reflectify.IsCollectionType(typeof(string[])));
    }

    [Fact]
    public void IsCollectionType_Dictionary_ReturnsTrue()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        Assert.True(reflectify.IsCollectionType(typeof(Dictionary<string, int>)));
    }

    [Fact]
    public void IsCollectionType_String_ReturnsFalse()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        Assert.False(reflectify.IsCollectionType(typeof(string)));
    }

    [Fact]
    public void IsCollectionType_NonEnumerableClass_ReturnsFalse()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        Assert.False(reflectify.IsCollectionType(typeof(MyDto)));
    }

    [Fact]
    public void IsCollectionType_IEnumerable_ReturnsTrue()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        Assert.True(reflectify.IsCollectionType(typeof(IEnumerable)));
    }

    [Fact]
    public void GetUnderlyingNonNullableType_NullableInt_ReturnsInt()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var result = reflectify.GetUnderlyingNonNullableType(typeof(int?));

        Assert.Equal(typeof(int), result);
    }

    [Fact]
    public void GetUnderlyingNonNullableType_NullableDecimal_ReturnsDecimal()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var result = reflectify.GetUnderlyingNonNullableType(typeof(decimal?));

        Assert.Equal(typeof(decimal), result);
    }

    [Fact]
    public void GetUnderlyingNonNullableType_NullableDateTime_ReturnsDateTime()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var result = reflectify.GetUnderlyingNonNullableType(typeof(DateTime?));

        Assert.Equal(typeof(DateTime), result);
    }

    [Fact]
    public void GetUnderlyingNonNullableType_NonNullableType_ReturnsSameType()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        Assert.Equal(typeof(int), reflectify.GetUnderlyingNonNullableType(typeof(int)));
        Assert.Equal(typeof(string), reflectify.GetUnderlyingNonNullableType(typeof(string)));
        Assert.Equal(typeof(MyDto), reflectify.GetUnderlyingNonNullableType(typeof(MyDto)));
    }

    [Fact]
    public void GetUnderlyingNonNullableType_ComplexNullable_ReturnsUnderlying()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration());
        var result = reflectify.GetUnderlyingNonNullableType(typeof(Guid?));

        Assert.Equal(typeof(Guid), result);
    }
}

public class MyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public abstract class AbstractBase;
