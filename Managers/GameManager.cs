using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Models;
using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace ExecutesPlugin.Managers
{
    public sealed class GameManager : BaseManager
    {
        public MapConfig _mapConfig = new();
        private Scenario? _currentScenario;
        private readonly QueueManager _queueManager;		
        private Dictionary<int, int> _playerRoundScores = new();
        private readonly int _consecutiveRoundWinsToScramble;
        private readonly bool _isScrambleEnabled;

        public const int ScoreForKill = 50;
        public const int ScoreForAssist = 25;
        public const int ScoreForDefuse = 50;

        private bool _scrambleNextRound;

        public GameManager(QueueManager queueManager)
        {
            _queueManager = queueManager;
            
            // TODO: Add a config option for this logic
            _consecutiveRoundWinsToScramble = 5;
            _isScrambleEnabled = true;
        }

        public bool LoadSpawns(string moduleDirectory, string map)
        {
            var fileName = $"{map}.json";

            // Path.Exists
            string _mapConfigDirectory = Path.Combine(moduleDirectory, "map_config");
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
            ParseMapConfigIdReferences(_mapConfig);

            Console.WriteLine($"-------------------------- Loaded {_mapConfig.Scenarios?.Count} executes config.");

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

			// If we find a scenario that has less spawns than active players, try again

			var players = Utilities.GetPlayers();


			foreach (var scenario in validScenarios)
			{
				var totalSpawnCount = (scenario.Spawns[CsTeam.Terrorist].Count + scenario.Spawns[CsTeam.CounterTerrorist].Count);

				if (totalSpawnCount < _queueManager.ActivePlayers.Count)
				{
					Console.WriteLine($"[Executes] Skipping \"{scenario.Name}\" because it has less spawns than players.");
					validScenarios.Remove(scenario);
				}
			}

			if (validScenarios.Count == 0)
			{
				Console.WriteLine($"[Executes] No valid scenarios found.");
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
            Console.WriteLine($"[Executes] Parsing map config id references for {mapConfig.Scenarios.Count} scenarios.");

            foreach (var scenario in mapConfig.Scenarios)
            {
                Console.WriteLine($"[Executes] Parsing scenario \"{scenario.Name}\" -> SpawnIds: \"{scenario.SpawnIds.Count}\"");

                scenario.Spawns[CsTeam.CounterTerrorist] = new List<Spawn>();
                scenario.Spawns[CsTeam.Terrorist] = new List<Spawn>();

                foreach (var spawnId in scenario.SpawnIds)
                {
                    var spawn = mapConfig.Spawns.First(x => x.Id == spawnId);

                    // TODO: Figure out why the IDE thinks spawn is never null
                    if (spawn != null)
                    {
                        scenario.Spawns[spawn.Team].Add(spawn);

                        Console.WriteLine($"[Executes] Added spawn \"{spawn.Id}\" to \"{spawn.Team}\"");
                    }
                    else
                    {
                        throw new Exception($"Error! spawn id \"{spawnId}\" does not exist!");
                    }
                }

                scenario.Grenades[CsTeam.CounterTerrorist] = new List<Grenade>();
                scenario.Grenades[CsTeam.Terrorist] = new List<Grenade>();

                foreach (var grenadeId in scenario.GrenadeIds)
                {
                    var grenade = mapConfig.Grenades.First(x => x.Id == grenadeId);

                    // TODO: Figure out why the IDE thinks grenade is never null
                    if (grenade != null)
                    {
                        scenario.Grenades[grenade.Team].Add(grenade);

                        Console.WriteLine($"[Executes] Added grenade \"{grenade.Name}\" to \"{grenade.Team}\"");
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

        public void ScrambleNextRound(CCSPlayerController? admin = null)
        {
            _scrambleNextRound = true;
            ChatHelpers.ChatMessageAll($"{admin?.PlayerName ?? "The server owner"} has set the teams to be scrambled next round.");
        }

        private void ScrambleTeams()
        {
            var shuffledActivePlayers = Helpers.Shuffle(_queueManager.ActivePlayers);

            var newTerrorists = shuffledActivePlayers.Take(_queueManager.GetTargetNumTerrorists()).ToList();
            var newCounterTerrorists = shuffledActivePlayers.Except(newTerrorists).ToList();

            SetTeams(newTerrorists, newCounterTerrorists);
        }

        public void ResetPlayerScores()
        {
            _playerRoundScores = new Dictionary<int, int>();
        }

        public void AddScore(CCSPlayerController player, int score)
        {
            if (!Helpers.IsValidPlayer(player) || player.UserId == null)
            {
                return;
            }

            var playerId = (int)player.UserId;

            if (!_playerRoundScores.TryAdd(playerId, score))
            {
                // Add to the player's existing score
                _playerRoundScores[playerId] += score;
            }
        }

        private int _consecutiveRoundsWon;

        public void TerroristRoundWin()
        {
            _consecutiveRoundsWon++;

            if (_consecutiveRoundsWon == _consecutiveRoundWinsToScramble)
            {
                ChatHelpers.ChatMessageAll($"The terrorists won \u0004{_consecutiveRoundWinsToScramble}\u0001 rounds in a row. ");

                _consecutiveRoundsWon = 0;
                ScrambleTeams();
            }
            else if (_consecutiveRoundsWon >= 3)
            {
                if (_isScrambleEnabled)
                {
                    ChatHelpers.ChatMessageAll($"The terrorists have won \u0004{_consecutiveRoundsWon}\u0001 rounds in a row - if they win \u0004{_consecutiveRoundWinsToScramble - _consecutiveRoundsWon}\u0001 more, teams will be scrambled. ");
                }
                else
                {
                    ChatHelpers.ChatMessageAll($"The terrorists have won \u0004{_consecutiveRoundsWon}\u0001 rounds in a row! ");
                }
            }
            else if (_scrambleNextRound)
            {
                _scrambleNextRound = false;
                _consecutiveRoundsWon = 0;
                ScrambleTeams();
            }
        }

        public void CounterTerroristRoundWin()
        {
            if (_consecutiveRoundsWon >= 3)
            {
                ChatHelpers.ChatMessageAll($"The CTs have ended a \u0004{_consecutiveRoundsWon}\u0001-round long win streak!");
            }

            _consecutiveRoundsWon = 0;

            var targetNumTerrorists = _queueManager.GetTargetNumTerrorists();
            var sortedCounterTerroristPlayers = GetSortedActivePlayers(CsTeam.CounterTerrorist);

            // Ensure that the players with the scores are set as new terrorists first.
            var newTerrorists = sortedCounterTerroristPlayers.Where(player => player.Score > 0)
                .Take(targetNumTerrorists)
                .ToList();

            if (newTerrorists.Count < targetNumTerrorists)
            {
                // Shuffle the other players with 0 score to ensure it's random who is swapped
                var playersLeft = Helpers.Shuffle(sortedCounterTerroristPlayers.Except(newTerrorists).ToList());
                newTerrorists.AddRange(playersLeft.Take(targetNumTerrorists - newTerrorists.Count));
            }

            if (newTerrorists.Count < targetNumTerrorists)
            {
                // If we still don't have enough terrorists
                newTerrorists.AddRange(
                    GetSortedActivePlayers(CsTeam.Terrorist)
                        .Take(targetNumTerrorists - newTerrorists.Count)
                );
            }

            newTerrorists
				.AddRange(sortedCounterTerroristPlayers
				.Where(player => player.Score > 0)
                .Take(targetNumTerrorists - newTerrorists.Count).ToList());

            var newCounterTerrorists = _queueManager.ActivePlayers.Except(newTerrorists).ToList();

            SetTeams(newTerrorists, newCounterTerrorists);
        }

        public void BalanceTeams()
        {
            List<CCSPlayerController> newTerrorists = new();
            List<CCSPlayerController> newCounterTerrorists = new();

            var currentNumTerrorist = Helpers.GetCurrentNumPlayers(CsTeam.Terrorist);
            var numTerroristsNeeded = _queueManager.GetTargetNumTerrorists() - currentNumTerrorist;

            if (numTerroristsNeeded > 0)
            {
                var sortedCounterTerroristPlayers = GetSortedActivePlayers(CsTeam.CounterTerrorist);

                newTerrorists = sortedCounterTerroristPlayers
					.Where(player => player.Score > 0)
                    .Take(numTerroristsNeeded).ToList();

                if (newTerrorists.Count < numTerroristsNeeded)
                {
                    var playersLeft = Helpers.Shuffle(sortedCounterTerroristPlayers.Except(newTerrorists).ToList());
                    newTerrorists.AddRange(playersLeft.Take(numTerroristsNeeded - newTerrorists.Count));
                }
            }

            var currentNumCounterTerroristAfterBalance = Helpers.GetCurrentNumPlayers(CsTeam.CounterTerrorist);
            var numCounterTerroristsNeeded =
                _queueManager.GetTargetNumCounterTerrorists() - currentNumCounterTerroristAfterBalance;

            if (numCounterTerroristsNeeded > 0)
            {
                var terroristsWithZeroScore = _queueManager.ActivePlayers
                    .Where(player =>
                        Helpers.IsValidPlayer(player)
                        && player.Team == CsTeam.Terrorist
                        && _playerRoundScores.GetValueOrDefault((int)player.UserId!, 0) == 0
                    )
                    .Except(newTerrorists)
                    .ToList();

                // Shuffle to avoid repetitive swapping of the same players
                newCounterTerrorists =
                    Helpers.Shuffle(terroristsWithZeroScore).Take(numCounterTerroristsNeeded).ToList();

                if (numCounterTerroristsNeeded > newCounterTerrorists.Count)
                {
                    // For remaining excess terrorists, move the ones with the lowest score to CT
                    newCounterTerrorists.AddRange(
                        _queueManager.ActivePlayers
                            .Except(newCounterTerrorists)
                            .Except(newTerrorists)
                            .Where(player => Helpers.IsValidPlayer(player) && player.Team == CsTeam.Terrorist)
                            .OrderBy(player => _playerRoundScores.GetValueOrDefault((int)player.UserId!, 0))
                            .Take(numTerroristsNeeded - newCounterTerrorists.Count)
                            .ToList()
                    );
                }
            }

            SetTeams(newTerrorists, newCounterTerrorists);
        }

        private List<CCSPlayerController> GetSortedActivePlayers(CsTeam? team = null)
        {
            return _queueManager.ActivePlayers
                .Where(Helpers.IsValidPlayer)
                .Where(player => team == null || player.Team == team)
                .OrderByDescending(player => _playerRoundScores.GetValueOrDefault((int)player.UserId!, 0))
                .ToList();
        }

        private void SetTeams(List<CCSPlayerController>? terrorists, List<CCSPlayerController>? counterTerrorists)
        {
            terrorists ??= new List<CCSPlayerController>();
            counterTerrorists ??= new List<CCSPlayerController>();

            foreach (var player in _queueManager.ActivePlayers.Where(Helpers.IsValidPlayer))
            {
                if (terrorists.Contains(player))
                {
                    player.SwitchTeam(CsTeam.Terrorist);
                }
                else if (counterTerrorists.Contains(player))
                {
                    player.SwitchTeam(CsTeam.CounterTerrorist);
                }
            }
        }
    }
}