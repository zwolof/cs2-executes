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

            var spawns = scenario.GetSpawns().ToDictionary(
                x => x.Key,
                x => x.Value
            );

            foreach(var player in Helpers.Shuffle(players))
            {
                if (!player.IsValidPlayer())
                {
                    continue;
                }

                var spawn = spawns[player.Team].First();
                spawns[player.Team].RemoveAt(0);

                player.MoveToSpawn(spawn);
            }
        }
    }
}