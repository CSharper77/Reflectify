# Reflectify

A lightweight .NET library providing caching reflection utilities with attribute inspection, method invocation helpers, and easy DI integration.

## Features

- **Cached reflection** — Type metadata, property attributes, and method attributes are lazily cached using `ConcurrentDictionary` for performance.
- **Extended type information** — `ExtendedTypeInfo`, `ExtendedPropertyInfo`, and `ExtendedMethodInfo` wrap standard reflection objects together with their custom attributes.
- **Attribute lookup** — Base class `AttributesInfo` provides `GetAttribute<TAttribute>()` to quickly retrieve attributes from any reflected member.
- **Method invocation** — `ExtendedMethodInfo` supports synchronous, generic-typed, and asynchronous (`InvokeAsync`, `InvokeVoidAsync`) method calls.
- **Type utilities** — Helpers like `IsCustomClassType`, `IsCollectionType`, and `GetUnderlyingNonNullableType`.
- **DI integration** — Extension method to register the service as a singleton in `IServiceCollection`.

## Installation

Add a package reference to your project:

```shell
dotnet add package Reflectify
```

Or via the NuGet Package Manager:

```shell
Install-Package Reflectify
```

## Usage

### Basic usage

```csharp
using Reflectify;

var reflection = new Reflection();

// Get extended type info (with custom attributes)
var typeInfo = reflection.GetTypeInfo(typeof(MyClass));
var attr = typeInfo.GetAttribute<MyCustomAttribute>();

// Get properties with their attributes
var properties = reflection.GetProperties(typeof(MyClass));
foreach (var prop in properties)
{
    var displayAttr = prop.GetAttribute<DisplayNameAttribute>();
    Console.WriteLine($"{prop.PropertyInfo.Name} - {displayAttr?.DisplayName}");
}

// Get methods with their attributes
var methods = reflection.GetMethods(typeof(MyClass));
foreach (var method in methods)
{
    var authAttr = method.GetAttribute<AuthorizeAttribute>();
    Console.WriteLine($"{method.Method.Name} - Authorized: {authAttr != null}");
}

// Invoke a method
var sampleMethod = reflection.GetMethod(typeof(MyClass), "DoWork");
sampleMethod?.Invoke(instance, arg1, arg2);

// Invoke a method asynchronously
var asyncMethod = reflection.GetMethod(typeof(MyService), "ProcessAsync");
var result = await asyncMethod?.InvokeAsync(serviceInstance);

// Type utilities
bool isClass = reflection.IsCustomClassType(typeof(MyClass));
bool isCollection = reflection.IsCollectionType(typeof(List<int>));
Type nonNullable = reflection.GetUnderlyingNonNullableType(typeof(int?)); // returns int
```

### Using with instances (generic overloads)

```csharp
var obj = new MyClass();

var typeInfo = reflection.GetTypeInfo(obj);
var properties = reflection.GetProperties(obj);
var property = reflection.GetProperty(obj, "Name");
var methods = reflection.GetMethods(obj);
var method = reflection.GetMethod(obj, "Execute");
```

## Dependency Injection

Register `IReflection` in your service collection:

```csharp
using Reflectify;

var services = new ServiceCollection();

// Option 1: Using the extension method
services.AddReflectionHelper();

// Option 2: Manual registration
services.AddSingleton<IReflection, Reflection>();
```

Then inject `IReflection` wherever needed:

```csharp
public class MyService
{
    private readonly IReflection _reflection;

    public MyService(IReflection reflection)
    {
        _reflection = reflection;
    }

    public void AnalyzeType<T>()
    {
        var typeInfo = _reflection.GetTypeInfo<T>();
        var properties = _reflection.GetProperties<T>();
    }
}
```

## API Overview

| Interface Method | Description |
|---|---|
| `GetProperties(Type)` | Returns all properties with their custom attributes |
| `GetProperty(Type, string)` | Returns a single property by name |
| `GetMethods(Type)` | Returns all methods with their custom attributes |
| `GetMethod(Type, string)` | Returns a single method by name |
| `GetTypeInfo(Type)` | Returns extended type info including custom attributes |
| `IsCustomClassType(Type)` | Returns `true` if the type is a class (excluding `string`) |
| `IsCollectionType(Type)` | Returns `true` if the type implements `IEnumerable` (excluding `string`) |
| `GetUnderlyingNonNullableType(Type)` | Unwraps `Nullable<T>` to `T`, otherwise returns the original type |

## Project Structure

```
Reflectify/
├── src/
│   ├── IReflection.cs          — Service interface
│   ├── Reflection.cs           — Default implementation with caching
│   ├── Extensions.cs           — DI registration helper
│   ├── AttributesInfo.cs       — Base class with attribute lookup
│   ├── ExtendedTypeInfo.cs     — Type + attributes wrapper
│   ├── ExtendedPropertyInfo.cs — PropertyInfo + attributes wrapper
│   ├── ExtendedMethodInfo.cs   — MethodInfo + attributes wrapper with invocation helpers
│   └── Reflectify.csproj       — Project file (net10.0)
└── README.md
```
