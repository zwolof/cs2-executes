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

            Console.WriteLine($"-------------------------- Loaded {_mapConfig.Scenarios?.Count} executes config.");
            
            ParseMapConfigIdReferences(_mapConfig);
            
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

        public void ParseMapConfigIdReferences(MapConfig mapConfig)
        {
            foreach (var scenario in mapConfig.Scenarios)
            {
                foreach (var spawnId in scenario.SpawnIds)
                {
                    var spawn = mapConfig.Spawns.First(x => x.Id == spawnId);
                    
                    // TODO: Figure out why the IDE thinks spawn is never null
                    if (spawn != null)
                    {
                        scenario.Spawns.Add(spawn);
                    }
                    else
                    {
                        throw new Exception($"Error! spawn id \"{spawnId}\" does not exist!");
                    }
                }
                
                foreach (var grenadeId in scenario.GrenadeIds)
                {
                    var grenade = mapConfig.Grenades.First(x => x.Id == grenadeId);
                    
                    // TODO: Figure out why the IDE thinks grenade is never null
                    if (grenade != null)
                    {
                        scenario.Grenades.Add(grenade);
                    }
                    else
                    {
                        throw new Exception($"Error! grenade id \"{grenadeId}\" does not exist!");
                    }
                }
            }
        }

        public Scenario GetCurrentScenario()
        {
            // TODO: Implement this properly
            return _currentScenario ?? throw new Exception("No current scenario");
        }
    }
}