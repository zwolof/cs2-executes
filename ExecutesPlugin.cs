using System.Diagnostics;
using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;
using ExecutesPlugin.Managers;
using ExecutesPlugin.Memory;
using ExecutesPlugin.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ExecutesPlugin
{
    [MinimumApiVersion(147)]
    public class ExecutesPlugin : BasePlugin
    {
        private const string Version = "0.0.1";
        
        #region Plugin Info
        public override string ModuleName => "Executes Plugin";
        public override string ModuleAuthor => "zwolof, B3none";
        public override string ModuleDescription => "Community executes for CS2.";
        public override string ModuleVersion => Version;
        #endregion

        private readonly GameManager _gameManager;
        private readonly SpawnManager _spawnManager;
        private readonly GrenadeManager _grenadeManager;
        
        private CsTeam _lastRoundWinner = CsTeam.None;
		public bool _IsEditMode = false;

        public ExecutesPlugin(GameManager gameManager, SpawnManager spawnManager, GrenadeManager grenadeManager)
        {
            _gameManager = gameManager;
            _spawnManager = spawnManager;
			_grenadeManager = grenadeManager;
        }

        public override void Load(bool hotReload)
        {
            RegisterListener<Listeners.OnMapStart>(OnMapStart);

			SmokeFunctions.CSmokeGrenadeProjectile_CreateFunc.Hook(OnSmokeGrenadeProjectileCreate, HookMode.Pre);
			
			AddCommandListener("jointeam", OnCommandJoinTeam);
			
            Console.WriteLine("[Executes] ----------- CS2 Executes loaded -----------");

            if (hotReload)
            {
                OnMapStart(Server.MapName);
            }
        }

        public override void Unload(bool hotReload)
        {
            RemoveListener("OnMapStart", OnMapStart);

            Console.WriteLine("[Executes] ----------- CS2 Executes unloaded -----------");
        }

        // private void OnSmokeGrenadeProjectileCreate(
		// 	IntPtr position,
		// 	IntPtr angle,
		// 	IntPtr velocity,
		// 	IntPtr angVelocity,
		// 	IntPtr pOwner,
		// 	IntPtr weaponInfo,
		// 	CsTeam team
		// )
        private HookResult OnSmokeGrenadeProjectileCreate(DynamicHook hook)
		{

			if (!_IsEditMode) return HookResult.Continue;
			
			Server.PrintToChatAll("[Executes] smokegrenade_projectile created [Pre].");

			var position = hook.GetParam<IntPtr>(0);
			var angle = hook.GetParam<IntPtr>(1);
			var velocity = hook.GetParam<IntPtr>(2);
			var angVelocity = hook.GetParam<IntPtr>(3);
			var pOwner = hook.GetParam<IntPtr>(4);
			var weaponInfo = hook.GetParam<IntPtr>(5);
			var team = hook.GetParam<CsTeam>(6);

			var grenade = new Grenade
			{
				Type = EGrenade.Smoke,
				Position = new Vector(position),
				Angle = new QAngle(angle),
				Velocity = new Vector(velocity),
				Team = team,
			};

			Server.PrintToChatAll(JsonSerializer.Serialize(grenade)); 
			Server.PrintToChatAll("[Executes] smokegrenade_projectile created [Post].");
			Server.PrintToChatAll($"[Executes] angVelocity: {new Vector(angVelocity).ToString()}");

			return HookResult.Continue;
		}

		private void OnMapStart(string mapName)
		{
			var loaded = _gameManager.LoadSpawns(ModuleDirectory, mapName);

			if (!loaded)
			{
				Console.WriteLine("[Executes] Failed to load spawns.");
				return;
			}
			
			Helpers.ExecuteExecutesConfiguration(ModuleDirectory);
		}

		[ConsoleCommand("css_debug", "Reloads the scenarios from the map config")]
		public void OnToggleDebugCommand(CCSPlayerController? player, CommandInfo commandInfo)
		{
			_IsEditMode = !_IsEditMode;

            player?.ChatMessage($"Debug mode is now {_IsEditMode}");
		}

		[ConsoleCommand("css_reloadscenarios", "Reloads the scenarios from the map config")]
		public void ReloadScenarios(CCSPlayerController? player, CommandInfo commandInfo)
		{
			var loaded = _gameManager.LoadSpawns(ModuleDirectory, Server.MapName);

            if (!loaded)
            {
                Console.WriteLine("[Executes] Failed to load spawns.");
            }
		}

		[ConsoleCommand("css_addspawn", "Adds a spawn point to the map")]
		[CommandHelper(
			minArgs: 2,
			usage: "[T/CT] [A/B]",
			whoCanExecute: CommandUsage.CLIENT_ONLY
		)]
		public void OnCommandAddSpawn(CCSPlayerController? player, CommandInfo commandInfo)
		{
			if (!player.IsValidPlayer())
			{
				commandInfo.ReplyToCommand("[Executes] You must be a player to execute this command.");
				return;
			}

			Debug.Assert(player != null, "player != null");
			
			var team = commandInfo.GetArg(1).ToUpper();

			if (team != "T" && team != "CT")
			{
				commandInfo.ReplyToCommand($"[Executes] You must specify a team [T / CT] - [Value: {team}].");
				return;
			}

			var bombsite = commandInfo.GetArg(2).ToUpper();

			if (bombsite != "A" && bombsite != "B")
			{
				commandInfo.ReplyToCommand($"[Executes] You must specify a bombsite [A / B] - [Value: {bombsite}].");
				return;
			}

			var spawn = new Spawn
			{
				Id = 0,
				Name = "Spawn",
				Position = player.PlayerPawn.Value!.AbsOrigin,
				Angle = player.PlayerPawn.Value.EyeAngles,
				Team = team == "T" ? CsTeam.Terrorist : CsTeam.CounterTerrorist,
				Type = ESpawnType.SPAWNTYPE_NORMAL
			};

			player.PrintToConsole("Latest spawn:");
			player.PrintToConsole("---------------------------");
			player.PrintToConsole(JsonSerializer.Serialize(spawn));
			player.PrintToConsole("---------------------------");

			commandInfo.ReplyToCommand("[Executes] Printed into console.");
		}

		[ConsoleCommand("css_addgrenade", "Adds a grenade to the map config")]
		[CommandHelper(
			minArgs: 2,
			usage: "[T/CT] [A/B]",
			whoCanExecute: CommandUsage.CLIENT_ONLY
		)]
		[RequiresPermissions("@css/root")]
		public void OnCommandAddGrenade(CCSPlayerController? player, CommandInfo commandInfo)
		{
			if (!player.IsValidPlayer())
			{
				commandInfo.ReplyToCommand("[Executes] You must be a player to execute this command.");
				return;
			}
			
			var team = commandInfo.GetArg(1).ToUpper();

			if (team != "T" && team != "CT")
			{
				commandInfo.ReplyToCommand($"[Executes] You must specify a team [T / CT] - [Value: {team}].");
				return;
			}

			var bombsite = commandInfo.GetArg(2).ToUpper();

			if (bombsite != "A" && bombsite != "B")
			{
				commandInfo.ReplyToCommand($"[Executes] You must specify a bombsite [A / B] - [Value: {bombsite}].");
				return;
			}

			Debug.Assert(player != null, "player != null");
			Debug.Assert(player.PlayerPawn != null, "player.PlayerPawn != null");
			Debug.Assert(player.PlayerPawn.Value != null, "player.PlayerPawn.Value != null");
			
			var grenade = new Grenade
			{
				Id = 0,
				Name = "x to y",
				Type = EGrenade.Smoke,
				Position = player.PlayerPawn.Value.AbsOrigin,
				Angle = player.PlayerPawn.Value.EyeAngles,
				Velocity = new Vector(-431.161926f, -115.314392f, 506.386200f),
				Delay = 0.0f,
			};

			player.PrintToConsole("Latest grenade:");
			player.PrintToConsole("---------------------------");
			player.PrintToConsole(JsonSerializer.Serialize(grenade));
			player.PrintToConsole("---------------------------");
		}
		
		[ConsoleCommand("css_scramble", "Sets teams to scramble on the next round.")]
		[ConsoleCommand("css_scrambleteams", "Sets teams to scramble on the next round.")]
		[CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
		[RequiresPermissions("@css/admin")]
		public void OnCommandScramble(CCSPlayerController? player, CommandInfo commandInfo)
		{
			_gameManager.ScrambleNextRound(player);
		}
		
		[GameEventHandler]
		public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
		{
			Console.WriteLine("[Executes] EventHandler::OnRoundEnd");
			
			_lastRoundWinner = (CsTeam)@event.Winner;

			return HookResult.Continue;
		}

		[GameEventHandler]
		public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
		{
			var player = @event.Userid;

			if (!Helpers.IsValidPlayer(player) || !Helpers.IsPlayerConnected(player))
			{
				return HookResult.Continue;
			}

			// debug and check if the player is in the queue.
			Console.WriteLine($"[Executes] [{player.PlayerName}] Checking ActivePlayers.");
			if (!_gameManager.QueueManager.ActivePlayers.Contains(player))
			{
				Console.WriteLine(
					$"[Executes] [{player.PlayerName}] Checking player pawn {player.PlayerPawn.Value != null}.");
				if (player.PlayerPawn.Value != null && player.PlayerPawn.IsValid && player.PlayerPawn.Value.IsValid)
				{
					Console.WriteLine(
						$"[Executes] [{player.PlayerName}] player pawn is valid {player.PlayerPawn.IsValid} && {player.PlayerPawn.Value.IsValid}.");
					Console.WriteLine($"[Executes] [{player.PlayerName}] calling playerpawn.commitsuicide()");
					player.PlayerPawn.Value.CommitSuicide(false, true);
				}

				Console.WriteLine($"[Executes] [{player.PlayerName}] Player not in ActivePlayers, moving to spectator.");
				if (!player.IsBot)
				{
					Console.WriteLine($"[Executes] [{player.PlayerName}] moving to spectator.");
					player.ChangeTeam(CsTeam.Spectator);
				}

				return HookResult.Continue;
			}
			else
			{
				Console.WriteLine($"[Executes] [{player.PlayerName}] Player is in ActivePlayers.");
			}

			return HookResult.Continue;
		}
		
		[GameEventHandler]
		public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
		{
			var attacker = @event.Attacker;
			var assister = @event.Assister;

			if (Helpers.IsValidPlayer(attacker))
			{
				_gameManager.AddScore(attacker, GameManager.ScoreForKill);
			}

			if (Helpers.IsValidPlayer(assister))
			{
				_gameManager.AddScore(assister, GameManager.ScoreForAssist);
			}

			return HookResult.Continue;
		}

		[GameEventHandler]
		public HookResult OnBombDefused(EventBombDefused @event, GameEventInfo info)
		{
			var player = @event.Userid;

			if (Helpers.IsValidPlayer(player))
			{
				_gameManager.AddScore(player, GameManager.ScoreForDefuse);
			}

			return HookResult.Continue;
		}
        
        [GameEventHandler(HookMode.Pre)]
        public HookResult OnPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
        {
	        Console.WriteLine("[Executes] EventHandler::OnPlayerTeam");
	        
	        // Ensure all team join events are silent.
	        @event.Silent = true;

	        return HookResult.Continue;
        }

        private HookResult OnCommandJoinTeam(CCSPlayerController? player, CommandInfo commandInfo)
        {
	        if (
		        !Helpers.IsValidPlayer(player)
		        || commandInfo.ArgCount < 2
		        || !Enum.TryParse<CsTeam>(commandInfo.GetArg(1), out var toTeam)
	        )
	        {
		        return HookResult.Handled;
	        }

	        var fromTeam = player!.Team;

	        Console.WriteLine($"[Executes] [{player.PlayerName}] {fromTeam} -> {toTeam}");

	        _gameManager.QueueManager.DebugQueues(true);
	        var response = _gameManager.QueueManager.PlayerJoinedTeam(player, fromTeam, toTeam);
	        _gameManager.QueueManager.DebugQueues(false);

	        Console.WriteLine($"[Executes] [{player.PlayerName}] checking to ensure we have active players");
	        // If we don't have any active players, setup the active players and restart the game.
	        if (_gameManager.QueueManager.ActivePlayers.Count == 0)
	        {
		        Console.WriteLine($"[Executes] [{player.PlayerName}] clearing round teams to allow team changes");
		        _gameManager.QueueManager.ClearRoundTeams();

		        Console.WriteLine(
			        $"[Executes] [{player.PlayerName}] no active players found, calling QueueManager.Update()");
		        _gameManager.QueueManager.DebugQueues(true);
		        _gameManager.QueueManager.Update();
		        _gameManager.QueueManager.DebugQueues(false);

		        Helpers.RestartGame();
	        }

	        return response;
        }

        [GameEventHandler]
        public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnPlayerConnectFull");

            var player = @event.Userid;

            if (player == null)
            {
                Console.WriteLine("[Executes] Failed to get player.");
                return HookResult.Continue;
            }

            player.TeamNum = (int)CsTeam.Spectator;
            player.ForceTeamTime = 3600.0f;

            // Create a timer to do this as it would occasionally fire too early.
            AddTimer(1.0f, () => player.ExecuteClientCommand("teammenu"));

            // TODO: Add the player to the queue
            // _queueManager._queue.Enqueue(player);            

            return HookResult.Continue;
        }
      
        [GameEventHandler]
        public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnPlayerDisconnect");

            var player = @event.Userid;

            if (player == null)
            {
                Console.WriteLine("[Executes] Failed to get player.");
                return HookResult.Continue;
            }

	        _gameManager.QueueManager.RemovePlayerFromQueues(player);

            return HookResult.Continue;
        }
    
        [GameEventHandler]
        public HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnRoundFreezeEnd");

            return HookResult.Continue;
        }

        [GameEventHandler]
        public HookResult OnRoundPreStart(EventRoundPrestart @event, GameEventInfo info)
        {
            Console.WriteLine("[Executes] EventHandler::OnRoundPreStart");

			if (Helpers.IsWarmup())
			{
				Console.WriteLine("[Executes] Warmup detected, skipping.");
				return HookResult.Continue;
			}
            
			// Reset round teams to allow team changes.
			_gameManager.QueueManager.ClearRoundTeams();

			// Update Queue status
			Console.WriteLine($"[Executes] Updating queues...");
			_gameManager.QueueManager.DebugQueues(true);
			_gameManager.QueueManager.Update();
			_gameManager.QueueManager.DebugQueues(false);
			Console.WriteLine($"[Executes] Updated queues.");

			// Handle team swaps during round pre-start.
			switch (_lastRoundWinner)
			{
				case CsTeam.CounterTerrorist:
					Console.WriteLine($"[Executes] Calling CounterTerroristRoundWin()");
					_gameManager.CounterTerroristRoundWin();
					Console.WriteLine($"[Executes] CounterTerroristRoundWin call complete");
					break;

				case CsTeam.Terrorist:
					Console.WriteLine($"[Executes] Calling TerroristRoundWin()");
					_gameManager.TerroristRoundWin();
					Console.WriteLine($"[Executes] TerroristRoundWin call complete");
					break;
			}

			_gameManager.BalanceTeams();

			// Set round teams to prevent team changes mid round
			_gameManager.QueueManager.SetRoundTeams();

            
            // Attempt to get a random scenario from the game manager
            var scenario = _gameManager.GetRandomScenario();

            if (scenario == null)
            {
                Console.WriteLine("[Executes] Failed to get executes.");
                return HookResult.Continue;
            }
            
            return HookResult.Continue;
        }

        [GameEventHandler]
        public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
	        // TODO: FIGURE OUT WHY THE FUCK I NEED TO DO THIS
	        var weirdAliveSpectators = Utilities.GetPlayers()
		        .Where(x => x is { TeamNum: < (int)CsTeam.Terrorist, PawnIsAlive: true });
	        foreach (var weirdAliveSpectator in weirdAliveSpectators)
	        {
		        // I **think** it's caused by auto team balance being on, so turn it off
		        Server.ExecuteCommand("mp_autoteambalance 0");
		        weirdAliveSpectator.CommitSuicide(false, true);
	        }
	        
            Console.WriteLine("[Executes] EventHandler::OnRoundStart");

			if (Helpers.IsWarmup())
			{
				Console.WriteLine("[Executes] Warmup detected, skipping.");
				return HookResult.Continue;
			}
            
			// Clear the round scores
			_gameManager.ResetPlayerScores();
            
            // If we have a scenario then setup the players

			var currentScenario = _gameManager.GetCurrentScenario();
            _spawnManager.SetupSpawns(currentScenario);
            _grenadeManager.SetupGrenades(currentScenario);
            
            return HookResult.Continue;
        }
    }

    public class WithDependencyInjectionPluginServiceCollection : IPluginServiceCollection<ExecutesPlugin>
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
