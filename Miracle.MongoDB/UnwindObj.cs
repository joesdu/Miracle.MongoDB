using MongoDB.Bson.Serialization.Attributes;

namespace Miracle.MongoDB;
public class UnwindObj<T>
{
    [BsonElement("Obj")]
    public T? Obj { get; set; }//1.T as List<T>,use for Projection,2.T as single Object,use for MongoDB array field Unwind result
    [BsonElement("Count")]
    public int Count { get; set; }//when T as List<T>,record Count
    [BsonElement("Index")]
    public int Index { get; set; }//record array field element's index before Unwindss
}