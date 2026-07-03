public class DynamicAttributeInfo
{
    public Type AttributeType { get; set; }
    public object[] ConstructorArgs { get; set; } = Array.Empty<object>();

    public DynamicAttributeInfo(Type attributeType, params object[] constructorArgs)
    {
        AttributeType = attributeType;
        ConstructorArgs = constructorArgs;
    }
}
