namespace Miracle.MongoDB;
public class ConventionPackOptions
{
    protected readonly List<Type> ConvertObjectIdToStringTypes = new();
    public void AddConvertObjectIdToStringTypes(params Type[] types) => ConvertObjectIdToStringTypes.AddRange(types);
    public bool IsConvertObjectIdToStringType(Type type) => ConvertObjectIdToStringTypes.Contains(type);
}