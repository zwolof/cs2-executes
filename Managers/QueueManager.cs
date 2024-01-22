using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;

namespace CS2Executes.Managers
{
  public sealed class QueueManager : BaseManager
  {
    public readonly Queue<CCSPlayerController> _queue = new();
  }
}