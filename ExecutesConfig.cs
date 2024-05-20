using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace ExecutesPlugin;

public class DisableOtherBombsiteOverride
{
    [JsonPropertyName("Enabled")] public bool OverrideEnabled { get; set; } = false;

    [JsonPropertyName("Value")] public bool OverrideValue { get; set; } = false;

}

public class ExecutesConfig : BasePluginConfig
{
    [JsonPropertyName("DisableOtherBombsiteOverride")]
    public DisableOtherBombsiteOverride DisableOtherBombsiteOverride { get; set; } = new DisableOtherBombsiteOverride();
}