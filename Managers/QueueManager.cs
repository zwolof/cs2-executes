using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CS2Executes.Models;

namespace CS2Executes.Managers
{
    public sealed class QueueManager : BaseManager
    {
        public readonly ExecutesQueue<CCSPlayerController> _queue = new();
    }
}