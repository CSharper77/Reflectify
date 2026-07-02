using System.Reflection;

namespace Reflectify;

/// <summary>Provides reflection services for inspecting types, properties, methods and their attributes.</summary>
public interface IReflection
{
    /// <summary>Returns all properties with their attributes for the given type.</summary>
    List<ExtendedPropertyInfo>? GetProperties(Type type);

    /// <summary>Returns all properties with their attributes for the given instance type.</summary>
    List<ExtendedPropertyInfo>? GetProperties<T>(T type) where T : class;

    /// <summary>Returns a single property by name for the given type.</summary>
    ExtendedPropertyInfo? GetProperty(Type type, string name);

    /// <summary>Returns a single property by name for the given instance type.</summary>
    ExtendedPropertyInfo? GetProperty<T>(T type, string name) where T : class;

    /// <summary>Returns all methods with their attributes for the given type.</summary>
    List<ExtendedMethodInfo> GetMethods(Type type);

    /// <summary>Returns all methods with their attributes for the given instance type.</summary>
    List<ExtendedMethodInfo> GetMethods<T>(T type) where T : class;

    /// <summary>Returns a single method by name for the given type.</summary>
    ExtendedMethodInfo? GetMethod(Type type, string name);

    /// <summary>Returns a single method by name for the given instance type.</summary>
    ExtendedMethodInfo? GetMethod<T>(T type, string name) where T : class;

    /// <summary>Returns extended type information including custom attributes.</summary>
    ExtendedTypeInfo GetTypeInfo(Type type);

    /// <summary>Returns extended type information for the given instance type.</summary>
    ExtendedTypeInfo? GetTypeInfo<T>(T type) where T : class;

    /// <summary>Returns true if the type is a class (excluding string).</summary>
    bool IsCustomClassType(Type type);

    /// <summary>Returns true if the type implements IEnumerable (excluding string).</summary>
    bool IsCollectionType(Type type);

    /// <summary>Unwraps Nullable{T} to T, otherwise returns the original type.</summary>
    Type GetUnderlyingNonNullableType(Type type);
}