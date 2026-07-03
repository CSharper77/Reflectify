using Xunit;
using Reflectify;
using Reflectify.Models;

namespace Reflectify.Tests;

public class DetectionVisibilityTests
{
    [Fact]
    public void OnlyPublic_ExcludesPrivateProperties()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.OnlyPublic
        });

        var props = reflectify.GetProperties(typeof(VisibilityModel));

        Assert.NotNull(props);
        Assert.DoesNotContain(props, p => p.PropertyInfo.Name == "PrivateProp");
    }

    [Fact]
    public void OnlyPublic_IncludesPublicProperties()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.OnlyPublic
        });

        var props = reflectify.GetProperties(typeof(VisibilityModel));

        Assert.Contains(props!, p => p.PropertyInfo.Name == "PublicProp");
    }

    [Fact]
    public void All_IncludesPrivateProperties()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.All
        });

        var props = reflectify.GetProperties(typeof(VisibilityModel));

        Assert.NotNull(props);
        Assert.Contains(props, p => p.PropertyInfo.Name == "PrivateProp");
    }

    [Fact]
    public void All_IncludesPublicProperties()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.All
        });

        var props = reflectify.GetProperties(typeof(VisibilityModel));

        Assert.Contains(props!, p => p.PropertyInfo.Name == "PublicProp");
    }

    [Fact]
    public void OnlyPublic_ExcludesPrivateMethods()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.OnlyPublic
        });

        var methods = reflectify.GetMethods(typeof(VisibilityModel));

        Assert.DoesNotContain(methods, m => m.Method.Name == "PrivateMethod");
    }

    [Fact]
    public void OnlyPublic_IncludesPublicMethods()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.OnlyPublic
        });

        var methods = reflectify.GetMethods(typeof(VisibilityModel));

        Assert.Contains(methods, m => m.Method.Name == "PublicMethod");
    }

    [Fact]
    public void All_IncludesPrivateMethods()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.All
        });

        var methods = reflectify.GetMethods(typeof(VisibilityModel));

        Assert.Contains(methods, m => m.Method.Name == "PrivateMethod");
    }

    [Fact]
    public void All_IncludesPublicMethods()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.All
        });

        var methods = reflectify.GetMethods(typeof(VisibilityModel));

        Assert.Contains(methods, m => m.Method.Name == "PublicMethod");
    }

    [Fact]
    public void DefaultConfiguration_IsOnlyPublic()
    {
        var config = new ReflectifyConfiguration();

        Assert.Equal(DetectionVisibility.OnlyPublic, config.DetectionVisibility);
    }

    [Fact]
    public void OnlyPublic_ExcludesInternalProperties()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.OnlyPublic
        });

        var props = reflectify.GetProperties(typeof(VisibilityModel));

        Assert.DoesNotContain(props!, p => p.PropertyInfo.Name == "InternalProp");
    }

    [Fact]
    public void All_IncludesInternalProperties()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.All
        });

        var props = reflectify.GetProperties(typeof(VisibilityModel));

        Assert.Contains(props!, p => p.PropertyInfo.Name == "InternalProp");
    }

    [Fact]
    public void OnlyPublic_ExcludesProtectedMethods()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.OnlyPublic
        });

        var methods = reflectify.GetMethods(typeof(VisibilityModel));

        Assert.DoesNotContain(methods, m => m.Method.Name == "ProtectedMethod");
    }

    [Fact]
    public void All_IncludesProtectedMethods()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.All
        });

        var methods = reflectify.GetMethods(typeof(VisibilityModel));

        Assert.Contains(methods, m => m.Method.Name == "ProtectedMethod");
    }

    [Fact]
    public void GetProperty_ByName_WithAll_ReturnsPrivateProperty()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.All
        });

        var prop = reflectify.GetProperty(typeof(VisibilityModel), "PrivateProp");

        Assert.NotNull(prop);
        Assert.Equal("PrivateProp", prop!.PropertyInfo.Name);
    }

    [Fact]
    public void GetMethod_ByName_WithAll_ReturnsPrivateMethod()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.All
        });

        var method = reflectify.GetMethod(typeof(VisibilityModel), "PrivateMethod");

        Assert.NotNull(method);
        Assert.Equal("PrivateMethod", method!.Method.Name);
    }

    [Fact]
    public void GetProperty_ByName_WithOnlyPublic_DoesNotReturnPrivate()
    {
        var reflectify = new Reflectify(new ReflectifyConfiguration
        {
            DetectionVisibility = DetectionVisibility.OnlyPublic
        });

        var prop = reflectify.GetProperty(typeof(VisibilityModel), "PrivateProp");

        Assert.Null(prop);
    }
}

public class VisibilityModel
{
    public string PublicProp { get; set; } = "public";
    private string PrivateProp { get; set; } = "private";
    internal string InternalProp { get; set; } = "internal";

    public void PublicMethod() { }
    private void PrivateMethod() { }
    protected void ProtectedMethod() { }
}
