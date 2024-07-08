using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using JPP.Cedar.Core;
using JPP.StructuralAnalysis.Models;
using System;
using System.Text.Json.Serialization.Metadata;


namespace JPP.Cedar.Rosetta
{
    public class RosettaContext : IStorableObject<RosettaContext>
    {
        public GravityAnalysisModel Model { get; set; }

        private Document _rDoc;

        public static Guid GetSchemaId()
        {
            return new Guid("2ec4a23c-8056-49b6-8207-250a9e2d8568");
        }

        public static JsonTypeInfo<RosettaContext> GetTypeInfo()
        {
            return SerializationContext.Default.RosettaContext;
        }

        public static RosettaContext LoadOrCreate(Document rDoc)
        {
            var result = Load(rDoc) ?? new RosettaContext();
            if (result._rDoc is null)
                result._rDoc = rDoc;

            return result;

        }

        public static RosettaContext? Load(Document rDoc)
        {
            using FilteredElementCollector collector = new FilteredElementCollector(rDoc);
            var dataStorages = collector.OfClass(typeof(DataStorage));

            // Find setting data storage
            RosettaContext? context = null;

            foreach (DataStorage dataStorage in dataStorages)
            {
                context = dataStorage.LoadObject<RosettaContext>();
                if (context is not null)
                {
                    context._rDoc = rDoc;
                    return context;
                }
            }

            return context;
        }

        public void Save()
        {
            using FilteredElementCollector collector = new FilteredElementCollector(_rDoc);
            var dataStorages = collector.OfClass(typeof(DataStorage));

            DataStorage targetDataStorage = null;


            foreach (DataStorage dataStorage in dataStorages)
            {
                RosettaContext? context = dataStorage.LoadObject<RosettaContext>();
                if (context is not null)
                {
                    targetDataStorage = dataStorage;
                }
            }


            if (targetDataStorage is null)
            {
                targetDataStorage = DataStorage.Create(_rDoc);
            }

            targetDataStorage.StoreObject(this);
        }
    }
}
