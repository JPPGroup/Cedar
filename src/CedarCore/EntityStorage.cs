using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Text;
using System.Text.Json;

namespace JPP.Cedar.Core
{
    public static class EntityStorage
    {
        public static void StoreObject<T>(this Element element, T source) where T : class, IStorableObject<T>
        {
            var (schema, dataLocation) = Build(T.GetSchemaId(), typeof(T));
            var entity = new Entity(schema);
            // set the value for this entity

            var dataString = JsonSerializer.Serialize(source, T.GetTypeInfo());
            decimal megabyteSize = ((decimal)Encoding.Unicode.GetByteCount(dataString) / 1048576);
            if (megabyteSize < 16)
            {
                entity.Set<string>(dataLocation, dataString, UnitTypeId.Custom);
                element.SetEntity(entity); // store the entity in the element
            }
            else
            {
                throw new InvalidOperationException("Data exceeds storage limit");
            }
        }

        public static T? LoadObject<T>(this Element element) where T : class, IStorableObject<T>
        {
            var (schema, dataLocation) = Build(T.GetSchemaId(), typeof(T));

            Entity retrievedEntity = element.GetEntity(schema);

            if (!retrievedEntity.IsValid())
                return null;

            string retrievedData = retrievedEntity.Get<string>(dataLocation, UnitTypeId.Custom);


            return JsonSerializer.Deserialize(retrievedData, T.GetTypeInfo());
        }

        private static (Schema, Field) Build(Guid objectId, Type t)
        {
            SchemaBuilder schemaBuilder = new SchemaBuilder(objectId);
            schemaBuilder.SetReadAccessLevel(AccessLevel.Vendor); // allow anyone to read the object
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Vendor); // restrict writing to this vendor only
            schemaBuilder.SetVendorId("JPPUK"); // required because of restricted write-access
            schemaBuilder.SetSchemaName($"EntityStorage{t.Name.ToString()}");
            // create a field to store an XYZ
            FieldBuilder fieldBuilder = schemaBuilder.AddSimpleField("JsonData", typeof(string));
            fieldBuilder.SetDocumentation("Json representation of a complex object");
            Schema schema = schemaBuilder.Finish();

            Field dataLocation = schema.GetField("JsonData");

            return (schema, dataLocation);
        }
    }
}
