using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace Reflectify;

/// <summary>Default implementation of <see cref="IReflection"/> that caches type metadata and custom attributes.</summary>
public class Reflection : IReflection
{
    private ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, List<Attribute>>> TypePropertiesAndAttributes = new ();
    private ConcurrentDictionary<Type, List<Attribute>> TypeAttributes = new();
    private ConcurrentDictionary<Type, ConcurrentDictionary<MethodInfo, List<Attribute>>> TypeMethodsAndAttributes = new ();
    /// <summary>Cache of method parameter types keyed by MethodInfo.</summary>
    public ConcurrentDictionary<MethodInfo, List<Type>> MethodParameters = new ();

    #region Properties and Attributes
    
    private void InitializeTypePropertiesDictionary(Type type)
    {
        if (TypePropertiesAndAttributes.ContainsKey(type))
            return;
        
        var properties = type.GetProperties();
        
        // get class attributes 
        var attributes = type.GetCustomAttributes().ToList();
        TypeAttributes.TryAdd(type, attributes);
        TypePropertiesAndAttributes[type] = new ();
        // get properties
        foreach (var prop in properties)
        {
            var attrs = prop.GetCustomAttributes().ToList();
            
            TypePropertiesAndAttributes[type].TryAdd(prop, attrs);

        }
    }


    /// <summary>Returns extended type info (including custom attributes) for the given type.</summary>
    public ExtendedTypeInfo GetTypeInfo(Type type) 
    {
        InitializeTypePropertiesDictionary(type);
        var result = new ExtendedTypeInfo(type, TypeAttributes[type]);

        return result;
    }

    /// <summary>Returns extended type info for the given instance type.</summary>
    public ExtendedTypeInfo? GetTypeInfo<T>(T type) where T : class =>  GetTypeInfo(typeof(T));

    /// <summary>Returns extended type info for the generic type parameter.</summary>
    public ExtendedTypeInfo GetTypeInfo<T>() where T : class => GetTypeInfo(typeof(T)); 
    
    /// <summary>Returns all properties with attributes for the given type.</summary>
    public List<ExtendedPropertyInfo>? GetProperties(Type type)
    {
        InitializeTypePropertiesDictionary(type);
        
        if (TypePropertiesAndAttributes.TryGetValue(type, out var dict))
        {
            return dict.Keys.Select(w=> new ExtendedPropertyInfo(w, dict[w])).ToList();
        }

        return null;

    }
    
    /// <summary>Returns all properties with attributes for the given instance type.</summary>
    public List<ExtendedPropertyInfo>? GetProperties<T>(T type) where T : class => GetProperties(typeof(T));

    /// <summary>Returns a single property by name for the given type.</summary>
    public ExtendedPropertyInfo? GetProperty(Type type, string name)
    {
        var properties = GetProperties(type);
        return properties?.FirstOrDefault(x => x.PropertyInfo.Name == name);
        
    }
    
    /// <summary>Returns a single property by name for the given instance type.</summary>
    public ExtendedPropertyInfo? GetProperty<T>(T type, string name) where T : class =>  GetProperty(typeof(T), name);
    
    #endregion
    
    #region Methods and Attributes

    private void InitializeMethodsAndAttributes(Type type)
    {
        if (TypeMethodsAndAttributes.ContainsKey(type))
            return;
        
        TypeMethodsAndAttributes[type] = new ConcurrentDictionary<MethodInfo, List<Attribute>>();
        
        foreach (var methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static))
        {
            var attributes = methodInfo.GetCustomAttributes().ToList();
            TypeMethodsAndAttributes[type].TryAdd(methodInfo, attributes);
        }
    }
    
    /// <summary>Returns all methods with attributes for the given type.</summary>
    public List<ExtendedMethodInfo> GetMethods(Type type)
    {
        InitializeMethodsAndAttributes(type);

        return TypeMethodsAndAttributes[type].Keys.Select(w=> new ExtendedMethodInfo(w, TypeMethodsAndAttributes[type][w])).ToList();
    }
    
    /// <summary>Returns all methods with attributes for the given instance type.</summary>
    public List<ExtendedMethodInfo> GetMethods<T>(T type) where T : class => GetMethods(typeof(T));

    /// <summary>Returns a single method by name for the given type.</summary>
    public ExtendedMethodInfo? GetMethod(Type type, string name)
    {
        var methods = GetMethods(type);
        return methods?.FirstOrDefault(x => x.Method.Name == name);
    }
    
    /// <summary>Returns a single method by name for the given instance type.</summary>
    public ExtendedMethodInfo? GetMethod<T>(T type, string name) where T : class => GetMethod(typeof(T), name);
    
    #endregion
    
    /// <summary>Returns true if the type is a class (excluding string).</summary>
    public bool IsCustomClassType(Type type)
    {
        return type.IsClass
               && type != typeof(string);
    }
    
    /// <summary>Returns true if the type implements IEnumerable (excluding string).</summary>
    public bool IsCollectionType(Type type)
    {
        return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
    }
    
    /// <summary>Unwraps Nullable{T} to T, otherwise returns the original type.</summary>
    public Type GetUnderlyingNonNullableType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);

        if (underlyingType != null)
            return underlyingType;
        else
            return type;
    }
}