using System.Text.Json.Serialization;

namespace AutomationHUB.Shared.Configuration.DataProcessor;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "ProcessorType")]
[JsonDerivedType(typeof(StxEtxProcessorConfig), "StxEtx")]
public abstract class ByteDataProcessorConfig
{
    // Gemeinsame Feld-Mappings am Ende
    public List<FieldMapping> FieldMappings { get; set; } = [];
}

