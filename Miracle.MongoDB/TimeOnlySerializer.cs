using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Miracle.MongoDB;
internal class TimeOnlySerializer : StructSerializerBase<TimeOnly>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TimeOnly value)
    {
        var str=value.ToString("HH:mm:ss.fff");
        context.Writer.WriteString(str);
    }

    public override TimeOnly Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var ticks = context.Reader.ReadString();
        var dateTime = BsonUtils.ToLocalTime(DateTime.Parse($"0001-01-01 {ticks}"));
        return TimeOnly.FromDateTime(dateTime);
    }
}