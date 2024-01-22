using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Executes.Managers;
using CS2Executes.Memory;
using CS2Executes.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CS2Executes
{
    [MinimumApiVersion(140)]
    public partial class CS2ExecutesPlugin : BasePlugin
    {
		#region Plugin Info
        public override string ModuleName => "CS2 Executes";
        public override string ModuleAuthor => "zwolof, b3none";
        public override string ModuleDescription => "Brings executes to CS2";
        public override string ModuleVersion => "1.0.0";
		#endregion

		private readonly GameManager? _gameManager = null;

		public CS2ExecutesPlugin(GameManager gameManager) {
			_gameManager = gameManager;
		}

        public override void Load(bool hotReload)
        {
            RegisterEventHandler<EventPlayerConnectFull>(EventHandler.OnPlayerConnectFull);
            RegisterEventHandler<EventRoundStart>(EventHandler.OnRoundStart);
            RegisterEventHandler<EventRoundEnd>(EventHandler.OnRoundEnd);
            RegisterEventHandler<EventRoundFreezeEnd>(EventHandler.OnRoundFreezeEnd);

			RegisterListener<Listeners.OnMapStart>((string mapName) => 
			{
				var loaded = _gameManager?.LoadSpawns(mapName);

				if(loaded == null || !loaded.Value)
				{
					Console.WriteLine("[Executes] Failed to load spawns.");
					return;
				}
			});

			// SmokeFunctions.CSmokeGrenadeProjectile_CreateFunc.Hook((DynamicHook hook) =>
			// {

			// 	return HookResult.Continue;
			// }, HookMode.Post);

			AddCommand("css_spawnsmoke", "Creates a working smoke grenade", (player, commandInfo) => {
                if(player == null || player.PlayerPawn.Value == null || player.PlayerPawn.Value.AbsOrigin == null)
                {
					Console.WriteLine("[Executes] Failed to get player.");
                    return;
                }
            });

			Console.WriteLine("[Executes] ----------- CS2 Executes loaded -----------");
        }
    }

	public class WithDependencyInjectionPluginServiceCollection : IPluginServiceCollection<CS2ExecutesPlugin>
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<QueueManager>();
			services.AddSingleton<GameManager>();
			services.AddSingleton<GrenadeManager>();
			services.AddSingleton<ExecutesQueue<CCSPlayerController>>();

			Console.WriteLine("[Executes] ----------- CS2 Executes services loaded -----------");
		}
	}
}
