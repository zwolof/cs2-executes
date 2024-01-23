using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Executes.Models;

namespace CS2Executes.Managers
{
    public sealed class SpawnManager : BaseManager
    {
        public SpawnManager() { }

        public void SetupSpawns(Execute execute)
        {
            var players = Utilities.GetPlayers();

            if(!players.Any())
            {
                return;
            }
            // TODO: A BUNCH OF CHECKS

            var spawns = execute.GetSpawns().ToDictionary(
                x => x.Key,
                x => x.Value
            );

            foreach(var player in Helpers.Shuffle(players))
            {
                if(!player.IsValidPlayer())
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