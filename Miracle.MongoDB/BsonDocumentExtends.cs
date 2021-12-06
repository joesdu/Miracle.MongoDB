using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Miracle.MongoDB;
public static class BsonDocumentExtends
{
    /// <summary>
    /// get BsonValue from BsonDocument by hierarchicalNames
    /// </summary>
    /// <param name="ele">example:{rid:"aaa",info:{name:'xiaobai',gender:{k:'01',v:'男'}}}</param>
    /// <param name="hierarchicalNames">example:"info.gender.v"</param>
    /// <returns></returns>
    public static BsonValue? GetValueByHierarchicalNames(this BsonValue ele, string hierarchicalNames)
    {
        var names = hierarchicalNames.Split('.', ',', '_', '|');
        if (names.Length == 0) throw new("hierarchyNames is not correct");
        for (int i = 0; i < names.Length; i++)
        {
            if (ele.IsBsonDocument && ele.AsBsonDocument.Contains(names[i])) ele = ele.AsBsonDocument[names[i]];
            else return null;
        }
        return ele;
    }
    /// <summary>
    /// get object value from BsonValue
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static object? GetValue(this BsonValue value, Type? totype = null) => value.BsonType switch
    {
        BsonType.Array => value.AsBsonArray.ToArray().Select(x => x.GetValue()),
        BsonType.Boolean => value.AsBoolean,
        BsonType.DateTime => value.ToUniversalTime(),
        BsonType.Decimal128 => value.AsDecimal,
        BsonType.Document => totype is null ? value.ToJson() : BsonSerializer.Deserialize(value.ToBsonDocument(), totype),
        BsonType.Double => value.AsDouble,
        BsonType.Int32 => value.AsInt32,
        BsonType.Int64 => value.AsInt64,
        BsonType.Null => null,
        BsonType.ObjectId => value.AsString,
        BsonType.String => value.AsString,
        BsonType.Timestamp => value.AsString,
        _ => null
    };
}