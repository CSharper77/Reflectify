using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

public static class DynamicTypeBuilder
{
    public static Type CreateDynamicType(
        string className, 
        List<DynamicAttributeInfo> classAttributes, 
        List<DynamicPropertyInfo> properties)
    {
        // 1. Setup basic assembly scaffolding
        AssemblyName assemblyName = new AssemblyName("DynamicRuntimeAssembly");
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicRuntimeModule");
        TypeBuilder typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class);

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
}


/* Usage
// 1. Define Attributes for the Class
// Let's add [Description("Dynamic Order Entity")] to the class
var classAttributes = new List<DynamicAttributeInfo>
        {
            new DynamicAttributeInfo(typeof(DescriptionAttribute), "Dynamic Order Entity")
        };

// 2. Define Properties and their respective Attributes
var properties = new List<DynamicPropertyInfo>
        {
            // Property 1: OrderID (int) with no attributes
            new DynamicPropertyInfo("OrderID", typeof(int)),

            // Property 2: Notes (string) with [StringLength(100)] attached to it
            new DynamicPropertyInfo("Notes", typeof(string), new List<DynamicAttributeInfo>
            {
                new DynamicAttributeInfo(typeof(StringLengthAttribute), 100)
            })
        };

// 3. Generate the compiled Class Type at Runtime
Type generatedType = DynamicTypeBuilder.CreateDynamicType("Order", classAttributes, properties);

// --- VERIFICATION & INSPECTION ---
Console.WriteLine($"Generated Class: {generatedType.Name}\n");

// Inspect Class Attributes
var classDesc = generatedType.GetCustomAttribute<DescriptionAttribute>();
Console.WriteLine($"[Class Attribute] Description: \"{classDesc?.Description}\"");

// Inspect Property Attributes
PropertyInfo notesProperty = generatedType.GetProperty("Notes");
var stringLengthAttr = notesProperty.GetCustomAttribute<StringLengthAttribute>();
Console.WriteLine($"[Property Attribute] Notes Length Max: {stringLengthAttr?.MaximumLength}");

// 4. Instantiate and interact with it 
object orderInstance = Activator.CreateInstance(generatedType);
notesProperty.SetValue(orderInstance, "Express Shipping Requested");
var value = notesProperty.GetValue(orderInstance);
Console.WriteLine($"Assigned Property Value: {notesProperty.GetValue(orderInstance)}");
 * */