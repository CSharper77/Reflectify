namespace Reflectify.Models;

/// <summary>Wraps a <see cref="System.Type"/> together with its custom attributes.</summary>
public class ExtendedTypeInfo : AttributesInfo
{
    /// <summary>The underlying type.</summary>
    public Type Type { get; }

    /// <summary>Initializes the wrapper with the type and its attributes.</summary>
    public ExtendedTypeInfo(Type type, List<Attribute>? attributes): base(attributes)
    {
        Type = type;
    }
}