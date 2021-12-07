﻿using MongoDB.Bson.Serialization.Attributes;

namespace Miracle.MongoDB;
public class UnwindObj<T>
{
    /// <summary>
    /// 1.T as List<T>,use for Projection,
    /// 2.T as single Object,use for MongoDB array field Unwind result
    /// </summary>
    [BsonElement("Obj")]
    public T? Obj { get; set; }
    /// <summary>
    /// when T as List<T>,record Count
    /// </summary>
    [BsonElement("Count")]
    public int Count { get; set; }
    /// <summary>
    /// record array field element's index before Unwindss
    /// </summary>
    [BsonElement("Index")]
    public int Index { get; set; }
}