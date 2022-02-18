using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Miracle.MongoDB;
internal class DateOnlySerializer : StructSerializerBase<DateOnly>
{
    private static readonly TimeOnly zeroTimeComponent = new();

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateOnly value)
    {
        var dateTime = value.ToDateTime(zeroTimeComponent);
        var str = dateTime.ToString("yyyy-MM-dd");
        context.Writer.WriteString(str);
    }

    public override DateOnly Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var str = context.Reader.ReadString();
        var dateTime = BsonUtils.ToLocalTime(DateTime.Parse(str));
        return DateOnly.FromDateTime(dateTime);
    }
}