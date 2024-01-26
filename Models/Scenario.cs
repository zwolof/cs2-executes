using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;

namespace ExecutesPlugin.Models
{
    public class Scenario
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public EBombsite Bombsite { get; set; }
        public List<int> SpawnIds { get; set; } = new();
        public List<int> GrenadeIds { get; set; } = new();

        [JsonIgnore]
        public Dictionary<CsTeam, List<Spawn>> Spawns { get; set; } = new();

        [JsonIgnore] 
        public Dictionary<CsTeam, List<Grenade>> Grenades { get; set; } = new();
    }
}
