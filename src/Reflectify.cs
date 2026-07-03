using Reflectify.Models;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Reflectify;

/// <summary>Default implementation of <see cref="IReflectify"/> that caches type metadata and custom attributes.</summary>
public class Reflectify : IReflectify
{
    private ConcurrentDictionary<Type, ConcurrentDictionary<PropertyInfo, List<Attribute>>> TypePropertiesAndAttributes = new ();
    private ConcurrentDictionary<Type, List<Attribute>> TypeAttributes = new();
    private ConcurrentDictionary<Type, ConcurrentDictionary<MethodInfo, List<Attribute>>> TypeMethodsAndAttributes = new ();
    /// <summary>Cache of method parameter types keyed by MethodInfo.</summary>
    private ConcurrentDictionary<MethodInfo, List<Type>> MethodParameters = new ();

    public ReflectifyConfiguration ReflectifyConfiguration { get; }

    public BindingFlags ReflectifyBindingFlags=>    
        ReflectifyConfiguration.DetectionVisibility == DetectionVisibility.OnlyPublic
             ? BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static 
             : BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;

    public Reflectify(ReflectifyConfiguration reflectifyConfiguration)
    {
        ReflectifyConfiguration = reflectifyConfiguration;
    }

    #region Properties and Attributes

    private void InitializeTypePropertiesDictionary(Type type)
    {
        if (TypePropertiesAndAttributes.ContainsKey(type))
            return;
        
        var properties = type.GetProperties(ReflectifyBindingFlags);
        
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
        
        foreach (var methodInfo in type.GetMethods(ReflectifyBindingFlags))
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

    #region Dynamic Type Creation
    public Type CreateDynamicType(string dynamicAssemblyName, string className, List<DynamicAttributeInfo> classAttributes, List<DynamicPropertyInfo> properties, Type? baseType = null)
    {
        // 1. Setup basic assembly scaffolding
        AssemblyName assemblyName = new AssemblyName(dynamicAssemblyName);
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicRuntimeModule");
        TypeBuilder typeBuilder = moduleBuilder.DefineType(baseType is not null ? $"{className}_inherits_{baseType.Name}" : className, System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Class, baseType);

        // 2. Apply Class-Level Attributes
        foreach (var attr in classAttributes)
        {
            CustomAttributeBuilder classAttrBuilder = CreateAttributeBuilder(attr);
            if (classAttrBuilder != null)
            {
                typeBuilder.SetCustomAttribute(classAttrBuilder);
            }
        }

        // 3. Apply Property-Level Attributes and Build Fields/Properties
        foreach (var prop in properties)
        {
            // Create private backing field
            FieldBuilder fieldBuilder = typeBuilder.DefineField($"_{prop.PropertyName.ToLower()}", prop.PropertyType, FieldAttributes.Private);

            // Create public property
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(prop.PropertyName, PropertyAttributes.HasDefault, prop.PropertyType, null);

            // Attach attributes directly to the property
            foreach (var attr in prop.Attributes)
            {
                CustomAttributeBuilder propAttrBuilder = CreateAttributeBuilder(attr);
                if (propAttrBuilder != null)
                {
                    propertyBuilder.SetCustomAttribute(propAttrBuilder);
                }
            }

            // Standard Get/Set method flags
            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // --- Define Getter Method ---
            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod($"get_{prop.PropertyName}", getSetAttr, prop.PropertyType, Type.EmptyTypes);
            ILGenerator getIl = getMethodBuilder.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getMethodBuilder);

            // --- Define Setter Method ---
            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod($"set_{prop.PropertyName}", getSetAttr, null, new Type[] { prop.PropertyType });
            ILGenerator setIl = setMethodBuilder.GetILGenerator();
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);
            setIl.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }

        // 4. Compile type into operational IL
        return typeBuilder.CreateType();
    }

    // Helper to map your dynamic attribute configurations to Reflection's CustomAttributeBuilder
    private static CustomAttributeBuilder CreateAttributeBuilder(DynamicAttributeInfo attrInfo)
    {
        // Extract constructor argument types to find the exact constructor signature
        Type[] argTypes = new Type[attrInfo.ConstructorArgs.Length];
        for (int i = 0; i < attrInfo.ConstructorArgs.Length; i++)
        {
            argTypes[i] = attrInfo.ConstructorArgs[i].GetType();
        }

        ConstructorInfo? ctor = attrInfo.AttributeType.GetConstructor(argTypes);
        if (ctor == null)
        {
            throw new InvalidOperationException($"No matching constructor found for attribute {attrInfo.AttributeType.Name}");
        }

        return new CustomAttributeBuilder(ctor, attrInfo.ConstructorArgs);
    }
    #endregion
}