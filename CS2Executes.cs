using System.Diagnostics;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CS2Executes.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace CS2Executes
{
    [MinimumApiVersion(147)]
    public partial class CS2ExecutesPlugin : BasePlugin
    {
        #region Plugin Info
        public override string ModuleName => "CS2 Executes";
        public override string ModuleAuthor => "zwolof, b3none";
        public override string ModuleDescription => "Brings executes to CS2";
        public override string ModuleVersion => "1.0.0";
        #endregion

        private readonly GameManager? _gameManager = null;
        private readonly QueueManager? _queueManager = null;
        private readonly SpawnManager? _spawnManager = null;

        public CS2ExecutesPlugin(GameManager gameManager, QueueManager queueManager, SpawnManager spawnManager)
        {
            _gameManager = gameManager;
            _queueManager = queueManager;
            _spawnManager = spawnManager;
        }

        public override void Load(bool hotReload)
        {
            RegisterListener<Listeners.OnMapStart>((string mapName) => 
            {
                var loaded = _gameManager?.LoadSpawns(mapName);

                if(loaded == null || !loaded.Value)
                {
                    Console.WriteLine("[Executes] Failed to load spawns.");
                    return;
                }
            });

            Console.WriteLine("[Executes] ----------- CS2 Executes loaded -----------");
        }

        [GameEventHandler]
        public HookResult OnPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnPlayerTeam");
            @event.Silent = true;

            return HookResult.Continue;
        }

        [GameEventHandler]
        public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnPlayerConnectFull");

            Debug.Assert(_queueManager != null);

            var player = @event.Userid;

            if(player == null)
            {
                Console.WriteLine("[Executes] Failed to get player.");
                return HookResult.Continue;
            }

            player.ForceTeamTime = 3600.0f;

            // Add the player to the queue
            _queueManager._queue.Enqueue(player);            

            return HookResult.Continue;
        }
      
        [GameEventHandler]
        public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnPlayerDisconnect");

            Debug.Assert(_queueManager != null);

            var player = @event.Userid;

            if(player == null)
            {
                Console.WriteLine("[Executes] Failed to get player.");
                return HookResult.Continue;
            }

            // Remove the player from the queue if they disconnect
            _queueManager._queue.Drop(player);

            return HookResult.Continue;
        }
    
        [GameEventHandler]
        public HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnRoundFreezeEnd");

            return HookResult.Continue;
        }
    
        [GameEventHandler]
        public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnRoundStart");

            Debug.Assert(_gameManager != null);
            Debug.Assert(_spawnManager != null);

            // Attempt to get a random scenario from the game manager
            var scenario = _gameManager.GetRandomScenario();

            if(scenario == null)
            {
                Console.WriteLine("[Executes] Failed to get scenario.");
                return HookResult.Continue;
            }

            // If we have a scenario then setup the players
            _spawnManager.SetupSpawns(scenario);
            
            return HookResult.Continue;
        }
      
        [GameEventHandler]
        public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnRoundEnd");
            
            return HookResult.Continue;
        }
    }

    public class WithDependencyInjectionPluginServiceCollection : IPluginServiceCollection<CS2ExecutesPlugin>
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<QueueManager>();
            services.AddSingleton<GameManager>();
            services.AddSingleton<GrenadeManager>();
            services.AddSingleton<SpawnManager>();

            Console.WriteLine("[Executes] ----------- CS2 Executes services loaded -----------");
        }
    }
}
