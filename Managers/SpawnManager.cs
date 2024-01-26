using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Models;

namespace ExecutesPlugin.Managers
{
    public sealed class SpawnManager : BaseManager
    {
        public SpawnManager() { }

        public void SetupSpawns(Scenario scenario)
        {
            var players = Utilities.GetPlayers();

            if (!players.Any())
            {
				Console.WriteLine("[Executes] No players found.");
                return;
            }
			// TODO: A BUNCH OF CHECKS

			// DON'T LOOK AT THIS
			var spawns = new Dictionary<CsTeam, List<Spawn>>
			{
				{ CsTeam.Terrorist, scenario.Spawns[CsTeam.Terrorist].ToList() },
				{ CsTeam.CounterTerrorist, scenario.Spawns[CsTeam.CounterTerrorist].ToList() },
			};

			Console.WriteLine($"[Executes] {spawns[CsTeam.CounterTerrorist].Count} CT-Spawns loaded.");
			Console.WriteLine($"[Executes] {spawns[CsTeam.Terrorist].Count} T-Spawns loaded.");

            foreach(var player in players)
            {
                if (player.Team != CsTeam.Terrorist && player.Team != CsTeam.CounterTerrorist)
                {
					Console.WriteLine($"[Executes] We hit this \"{player.PlayerName}\"");
                    continue;
                }

				var randomSpawnIndex = new Random().Next(0, spawns[player.Team].Count);
				var randomSpawn = spawns[player.Team][randomSpawnIndex];

				spawns[player.Team].RemoveAt(randomSpawnIndex);
				player.MoveToSpawn(randomSpawn);

				Console.WriteLine($"[Executes] Random spawn is \"{randomSpawn.Name}\"");
            }
        }
    }
}