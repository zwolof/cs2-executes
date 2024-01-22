using CounterStrikeSharp.API.Modules.Utils;
using CS2Executes.Enums;

namespace CS2Executes.Models
{
    public class Execute
    {
        public string Name { get; set; }
        public EBombsite Bombsite { get; set; }
        public Dictionary<CsTeam, List<Spawn>> Spawns { get; set; }

        public Execute(string name, EBombsite bombsite, Dictionary<CsTeam, List<Spawn>> spawns)
        {
            Name = name;
            Bombsite = bombsite;
            Spawns = spawns;
        }
    }
}

// {
// 	"Name": "Test",
// 	"Bombsite": "A",
// 	"Spawns": {
//		"1": [
//			{
//				"Name": "Top mid",
//				"Team": 2,
//				"Position": {
//					"X": 0,
//					"Y": 0,
//					"Z": 0
//				},
//				"Angle": {
//					"X": 0,
//					"Y": 0,
//					"Z": 0
//				}
//			}
//		],
// 	}
// }