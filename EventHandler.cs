using CounterStrikeSharp.API.Core;
using CS2Executes.Managers;

namespace CS2Executes
{
  public class EventHandler
  {
    private readonly QueueManager _queueManager;
    public EventHandler(QueueManager queueManager)
    {
      _queueManager = queueManager;
    }

		public HookResult OnEventPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
    {
      Console.WriteLine("[Executes] EventHandler::OnEventPlayerTeam");
      @event.Silent = true;

      return HookResult.Continue;
    }

    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
      Console.WriteLine("[Executes] EventHandler::OnPlayerConnectFull");

      var player = @event.Userid;

      if(player == null)
      {
        Console.WriteLine("[Executes] Failed to get player.");
        return HookResult.Continue;
      }

      player.ForceTeamTime = 3600.0f;
      _queueManager._queue.Enqueue(player);

      return HookResult.Continue;
    }
    
    public HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
    {
      Console.WriteLine("[Executes] EventHandler::OnRoundFreezeEnd");

      return HookResult.Continue;
    }
    
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
      Console.WriteLine("[Executes] EventHandler::OnRoundStart");
      
      return HookResult.Continue;
    }
    
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
      Console.WriteLine("[Executes] EventHandler::OnRoundEnd");
      
      return HookResult.Continue;
    }
  }
}