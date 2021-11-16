using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Miracle.MongoDB
{
    /// <summary>
    /// map the [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId]
    /// </summary>
    public class StringObjectIdIdGeneratorConvention : ConventionBase, IPostProcessingConvention
    {
        public void PostProcess(BsonClassMap classMap)
        {
            var idMemberMap = classMap.IdMemberMap;
            if (idMemberMap is not null && idMemberMap.IdGenerator is null)
            {
                if (idMemberMap.MemberType == typeof(string)) _ = idMemberMap.SetIdGenerator(StringObjectIdGenerator.Instance).SetSerializer(new StringSerializer(BsonType.ObjectId));
            }
        }
    }
}