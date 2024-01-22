using CounterStrikeSharp.API.Core;
using CS2Executes.Managers;

namespace CS2Executes
{
    public class EventHandler
    {
        public static HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnPlayerConnectFull");

            return HookResult.Continue;
        }
        
        public static HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnRoundFreezeEnd");

            return HookResult.Continue;
        }
        
        public static HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnRoundStart");
            
            return HookResult.Continue;
        }
        
        public static HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnRoundEnd");
            
            return HookResult.Continue;
        }
    }
}