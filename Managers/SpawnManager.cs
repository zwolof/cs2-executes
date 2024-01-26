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

            foreach(var player in Helpers.Shuffle(players))
            {
				Console.WriteLine($"[Executes] YOUR FUCKING TEAM IS {player.Team}");

                if (player.Team != CsTeam.Terrorist && player.Team != CsTeam.CounterTerrorist)
                {
					Console.WriteLine($"[Executes] We hit this bs \"{player.PlayerName}\"");
                    continue;
                }

                // Since spawns are already shuffled, we can just take the first one
                var spawn = spawns[player.Team].First();
				Console.WriteLine($"[Executes] Spawn is \"{spawn.Name}\"");

                // Now we get rid of it so we don't use it again
                spawns[player.Team].Remove(spawn);

                player.MoveToSpawn(spawn);
            }
        }
    }
}