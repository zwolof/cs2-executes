using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Executes.Managers;
using CS2Executes.Memory;
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
				
				// EmitSound( const char *soundname, float soundtime = 0.0f, float *duration = NULL );
				var test = SmokeFunctions.EmitSound.Invoke(
					"Weapon_Knife.Hit",
					0.0f,
					1.5f
				);
				Server.PrintToChatAll($"[Executes] Executing EmitSound");


				// var vPos = player.PlayerPawn.Value.AbsOrigin;

				// vPos.Z += 10.0f;

				// SmokeFunctions.CSmokeGrenadeProjectile_CreateFunc.Invoke(
				// 	player.PlayerPawn.Value.AbsOrigin.Handle,
				// 	player.PlayerPawn.Value.EyeAngles.Handle,
				// 	player.PlayerPawn.Value.AbsVelocity.Handle,
				// 	player.PlayerPawn.Value.AngVelocity.Handle,
				// 	player.Pawn.Value!.Handle,
				// 	45,
				// 	player.TeamNum
				// );
				

				// smokegrenade_projectile.TeamNum = player.TeamNum;

				// smokegrenade_projectile.AbsOrigin.X = player.PlayerPawn.Value.AbsOrigin.X;
				// smokegrenade_projectile.AbsOrigin.Y = player.PlayerPawn.Value.AbsOrigin.Y;
				// smokegrenade_projectile.AbsOrigin.Z = player.PlayerPawn.Value.AbsOrigin.Z + 10.0f;

				// smokegrenade_projectile.DidSmokeEffect = false;

				// smokegrenade_projectile.SmokeDetonationPos.X = smokegrenade_projectile.AbsOrigin.X;
				// smokegrenade_projectile.SmokeDetonationPos.Y = smokegrenade_projectile.AbsOrigin.Y;
				// smokegrenade_projectile.SmokeDetonationPos.Z = smokegrenade_projectile.AbsOrigin.Z;

				// smokegrenade_projectile.SpawnTime = Server.TickCount;

				// smokegrenade_projectile.LastBounce = Server.TickCount + 1;
				// smokegrenade_projectile.LastThinkTick = Server.TickCount + 1;
				// smokegrenade_projectile.NextThinkTick = Server.TickCount + 1;
				// smokegrenade_projectile.SmokeEffectTickBegin = Server.TickCount + 10;
				// smokegrenade_projectile.Bounces = 0;
				// smokegrenade_projectile.SmokeColor.X = 256;
				// smokegrenade_projectile.SmokeColor.Y = 256;
				// smokegrenade_projectile.SmokeColor.Z = 256;
				
				// smokegrenade_projectile.DispatchSpawn();

				// smokegrenade_projectile.AcceptInput("InitializeSpawnFromWorld", player.PlayerPawn.Value!, player.PlayerPawn.Value!, "");
				// smokegrenade_projectile.DetonateTime = 0;
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

			Console.WriteLine("[Executes] ----------- CS2 Executes services loaded -----------");
		}
	}
}
