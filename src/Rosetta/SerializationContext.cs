using System.Text.Json.Serialization;

namespace JPP.Cedar.Rosetta
{
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
    [JsonSerializable(typeof(RosettaContext))]
    internal partial class SerializationContext : JsonSerializerContext
    {
    }
}
