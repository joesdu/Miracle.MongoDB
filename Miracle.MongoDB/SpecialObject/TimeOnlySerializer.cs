using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Miracle.MongoDB;
internal class TimeOnlySerializer : StructSerializerBase<TimeOnly>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TimeOnly value)
    {
        var str=value.ToString("HH:mm:ss");
        context.Writer.WriteString(str);
    }

    public override TimeOnly Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var ticks = context.Reader.ReadString();
        // 使用一个特殊的日子作为日期部分.得到一个本地化的DateTime类型.
        var dateTime = BsonUtils.ToLocalTime(DateTime.Parse($"1994-02-15 {ticks}"));
        return TimeOnly.FromDateTime(dateTime);
    }
}