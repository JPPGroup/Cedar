using System;
using System.Text.Json.Serialization.Metadata;

namespace JPP.Cedar.Core
{
    public interface IStorableObject<T> where T : class
    {
        static abstract Guid GetSchemaId();

        static abstract JsonTypeInfo<T> GetTypeInfo();
    }
}
