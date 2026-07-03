namespace Reflectify.Models;

/// <summary>Base class providing attribute lookup for reflected members.</summary>
public class AttributesInfo
{
    /// <summary>The list of custom attributes applied to the member.</summary>
    public List<Attribute>? Attributes { get; private set; }

    /// <summary>Initializes the store with the given attribute list.</summary>
    public AttributesInfo(List<Attribute>? attribute)
    {
        Attributes = attribute;
    }

    /// <summary>Returns the first attribute of the specified type, or null.</summary>
    public TAttribute? GetAttribute<TAttribute>() where TAttribute : Attribute
    {
        return (TAttribute?)Attributes?.FirstOrDefault(x => x is TAttribute);
    } 
}