using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;
using ExecutesPlugin.Models;
using Newtonsoft.Json;

namespace ExecutesPlugin.Managers
{
    public sealed class GameManager : BaseManager
    {
        private MapConfig _scenarios = new();
        private Scenario? _currentScenario;

        public GameManager() { }

        public bool LoadSpawns(string moduleDirectory, string map)
        {
            var fileName = $"{map}.json";
            var configExists = File.Exists(fileName);

            // Path.Exists
            string _mapConfigDirectory = Path.Combine(moduleDirectory, "map_configs");
            string _mapConfigPath = Path.Combine(_mapConfigDirectory, fileName);

            if (!File.Exists(_mapConfigPath))
            {
                Console.WriteLine($"[Executes] {fileName} does not exist.");
                return false;
            }

            var config = File.ReadAllText(fileName);

            var parsedConfig = JsonConvert.DeserializeObject<MapConfig>(config);

            if (parsedConfig == null)
            {
                Console.WriteLine($"[Executes] Failed to parse {fileName}");
                return false;
            }


            _scenarios = parsedConfig;

            Console.WriteLine($"-------------------------- Loaded {_scenarios.Scenarios?.Count} executes.");
            return true;
        }

        public Scenario? GetRandomScenario()
        {
            var counts = Helpers.GetPlayerCountDict();

            var validScenarios = _scenarios.Scenarios;
            // .Where(x =>
            // {
            //     var spawnIds = x.Spawns;
            //     var valid = true;

            //     foreach (var (team, spawnsList) in spawnIds)
            //     {
            //         if (spawnsList.Count > counts[team])
            //         {
            //             valid = false;
            //         }
            //     }

                // return valid;
            // }).ToList();

            if (!validScenarios!.Any())
            {
                _currentScenario = null;
                return null;
            }

            var random = Helpers.GetRandomInt(0, validScenarios!.Count);

            var current = validScenarios[random];
            
            _currentScenario = current;
            return current;
        }

        public Scenario GetCurrentScenario()
        {
            // TODO: Implement this properly
            return _currentScenario ?? throw new Exception("No current scenario");
        }
    }
}