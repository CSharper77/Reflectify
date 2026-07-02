namespace Reflectify;
using System.Reflection;

/// <summary>Wraps a <see cref="System.Reflection.PropertyInfo"/> together with its custom attributes.</summary>
public class ExtendedPropertyInfo : AttributesInfo
{
    /// <summary>The underlying reflection property information.</summary>
    public PropertyInfo PropertyInfo { get; }

    /// <summary>Initializes the wrapper with the property and its attributes.</summary>
    public ExtendedPropertyInfo(PropertyInfo propertyInfo, List<Attribute> attributes) : base(attributes)
    {
        PropertyInfo = propertyInfo;
    }
}