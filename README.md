# Reflectify

A .NET library that wraps reflection with caching, attribute inspection, method invocation helpers, and dynamic type creation.

## Install

```shell
dotnet add package Reflectify
```

## DI Registration

```csharp
using Reflectify.Extensions;

services.AddReflectify(config =>
{
    config.LifeTime = LifeTime.Singleton;        // Singleton | Scoped | Transient
    config.DetectionVisibility = DetectionVisibility.OnlyPublic; // OnlyPublic | All
});
```

Then inject `IReflectify`:

```csharp
public class MyService(IReflectify reflectify)
{
    public void Inspect(Type type)
    {
        var info = reflectify.GetTypeInfo(type);
        var props = reflectify.GetProperties(type);
        var methods = reflectify.GetMethods(type);
    }
}
```

## Usage

```csharp
using Reflectify;
using Reflectify.Models;

var reflectify = new Reflectify(new ReflectifyConfiguration());

// Get type info + custom attributes
var typeInfo = reflectify.GetTypeInfo(typeof(MyClass));
var attr = typeInfo.GetAttribute<MyCustomAttribute>();

// Get properties with attributes
var props = reflectify.GetProperties(typeof(MyClass));
var nameProp = reflectify.GetProperty(typeof(MyClass), "Name");
var displayAttr = nameProp?.GetAttribute<DisplayNameAttribute>();

// Get methods with attributes
var methods = reflectify.GetMethods(typeof(MyClass));
var method = reflectify.GetMethod(typeof(MyClass), "Execute");

// Invoke methods synchronously or asynchronously
var result = method?.Invoke(instance, arg1, arg2);
var asyncResult = await asyncMethod.InvokeAsync(serviceInstance);
var typed = await asyncMethod.InvokeAsync<string>(serviceInstance);

// Generic overloads with instances
var obj = new MyClass();
reflectify.GetProperties(obj);
reflectify.GetMethod(obj, "DoWork");

// Type utilities
reflectify.IsCustomClassType(typeof(MyDto));         // true
reflectify.IsCollectionType(typeof(List<int>));      // true
reflectify.GetUnderlyingNonNullableType(typeof(int?)); // System.Int32

// Create dynamic types at runtime
var dynamicType = reflectify.CreateDynamicType(
    "MyAssembly", "Person",
    [new DynamicAttributeInfo(typeof(ObsoleteAttribute), "old")],
    [new DynamicPropertyInfo("Name", typeof(string))]
);
```
