using ExecutesPlugin.Models;
using System.Text.Json;

namespace ExecutesPlugin.Managers
{
    public sealed class GameManager : BaseManager
    {
        private MapConfig _mapConfig = new();
        private Scenario? _currentScenario;

        public GameManager() { }

        public bool LoadSpawns(string moduleDirectory, string map)
        {
            var fileName = $"{map}.json";

            // Path.Exists
            string _mapConfigDirectory = Path.Combine(moduleDirectory, "map_configs");
            string _mapConfigPath = Path.Combine(_mapConfigDirectory, fileName);

            Console.WriteLine($"[Executes] Loading \"{_mapConfigPath}\"");

            if (!File.Exists(_mapConfigPath))
            {
                Console.WriteLine($"[Executes] {fileName} does not exist.");
                return false;
            }

            var config = File.ReadAllText(_mapConfigPath);

            var parsedConfig = JsonSerializer.Deserialize<MapConfig>(config);

            if (parsedConfig == null)
            {
                Console.WriteLine($"[Executes] Failed to parse {fileName}");
                return false;
            }

            _mapConfig = parsedConfig;

            Console.WriteLine($"-------------------------- Loaded {_mapConfig.Scenarios?.Count} executes.");
            return true;
        }

        public Scenario? GetRandomScenario()
        {
            Console.WriteLine("Calling GameManager::GetRandomScenario()");
            Console.WriteLine($"There are {_mapConfig.Scenarios.Count} scenarios loaded.");

            var validScenarios = _mapConfig.Scenarios;

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