using System;
using MongoDB.Bson;

namespace RaveCalcApiCommander.Abstraction
{
    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }

        public DateTime CreatedAt => Id.CreationTime;
    }
}
