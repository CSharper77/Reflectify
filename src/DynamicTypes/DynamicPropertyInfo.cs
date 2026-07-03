public class DynamicPropertyInfo
{
    public string PropertyName { get; set; }
    public Type PropertyType { get; set; }
    public List<DynamicAttributeInfo> Attributes { get; set; } = new();

    public DynamicPropertyInfo(string propertyName, Type propertyType, List<DynamicAttributeInfo> attributes = null)
    {
        PropertyName = propertyName;
        PropertyType = propertyType;
        if (attributes != null) Attributes = attributes;
    }
}
