using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;

namespace ExecutesPlugin.Models
{
    public class Scenario
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public EBombsite Bombsite { get; set; }
        public List<int>? Spawns { get; set; }
        public List<int>? Grenades { get; set; }

        public Dictionary<CsTeam, List<Spawn>> GetSpawns()
        {
            // TODO: Convert List<int> to Dictionary<CsTeam, List<Spawn>>
            return new Dictionary<CsTeam, List<Spawn>>();
        }

        public Dictionary<CsTeam, List<Grenade>> GetGrenades()
        {
            // TODO: Convert List<int> to Dictionary<CsTeam, List<Grenade>>
            return new Dictionary<CsTeam, List<Grenade>>();
        }
    }
}
