using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;
using ExecutesPlugin.Memory;
using ExecutesPlugin.Models;
using Newtonsoft.Json;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace ExecutesPlugin.Managers
{
    public sealed class GameManager : BaseManager
    {
        private List<Scenario> _executes = new();

        public GameManager() { }

        public bool LoadSpawns(string map)
        {
            var fileName = $"{map}.json";
            var configExists = File.Exists(fileName);

            if (!configExists)
            {
                Console.WriteLine($"[Executes] {fileName} does not exist.");
                return false;
            }

            var config = File.ReadAllText(fileName);

            var parsedConfig = JsonConvert.DeserializeObject<List<Scenario>>(config);

            if (parsedConfig == null)
            {
                Console.WriteLine($"[Executes] Failed to parse {fileName}");
                return false;
            }

            _executes = parsedConfig;

            Console.WriteLine($"-------------------------- Loaded {_executes.Count} executes.");
            return true;
        }

        public Scenario? GetRandomScenario()
        {
            // TODO: Implement this
            // TODO: Set the current scenario
            return null;
        }

        public Scenario GetCurrentScenario()
        {
            // TODO: Implement this properly
            return new Scenario("test", EBombsite.A, new Dictionary<CsTeam, List<Spawn>>(), new List<Grenade>());
        }
    }
}