using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;
using ExecutesPlugin.Models;
using Newtonsoft.Json;

namespace ExecutesPlugin.Managers
{
    public sealed class GameManager : BaseManager
    {
        private List<Scenario> _scenarios = new();
        private Scenario? _currentScenario;

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

            _scenarios = parsedConfig;

            Console.WriteLine($"-------------------------- Loaded {_scenarios.Count} executes.");
            return true;
        }

        public Scenario? GetRandomScenario()
        {
            var counts = Helpers.GetPlayerCountDict();

            var validScenarios = _scenarios.Where(
                scen => scen.GetSpawns().All(y => y.Value.Count >= counts[y.Key])
            ).ToList();

            if (!validScenarios.Any())
            {
                _currentScenario = null;
                return null;
            }

            var random = Helpers.GetRandomInt(0, validScenarios.Count);

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