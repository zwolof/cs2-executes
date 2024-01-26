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
                return;
            }
			// TODO: A BUNCH OF CHECKS

			var spawns = scenario.Spawns;

			Console.WriteLine($"[Executes] {spawns[CsTeam.CounterTerrorist]} ct-spawns loaded.");
			Console.WriteLine($"[Executes] {spawns[CsTeam.Terrorist]} t-spawns loaded.");

            foreach(var player in Helpers.Shuffle(players))
            {
                if (!player.IsValidPlayer())
                {
                    continue;
                }

                // Since spawns are already shuffled, we can just take the first one
                var spawn = spawns[player.Team].First();

                // Now we get rid of it so we don't use it again
                spawns[player.Team].Remove(spawn);

                player.MoveToSpawn(spawn);
            }
        }
    }
}