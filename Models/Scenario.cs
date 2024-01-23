using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;

namespace ExecutesPlugin.Models
{
    public class Scenario
    {
        public string Name { get; set; }
        public EBombsite Bombsite { get; set; }
        public Dictionary<CsTeam, List<Spawn>> Spawns { get; set; }
        public List<Grenade> Grenades { get; set; }

        public Scenario(string name, EBombsite bombsite, Dictionary<CsTeam, List<Spawn>> spawns, List<Grenade> grenades)
        {
            Name = name;
            Bombsite = bombsite;
            Spawns = spawns;
            Grenades = grenades;
        }

        public Dictionary<CsTeam, List<Spawn>> GetSpawns()
        {
            return Spawns;
        }
    }
}
