using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Models.JsonConverters;

namespace ExecutesPlugin;

public static class Helpers
{
	internal static readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		WriteIndented = true,
		Converters =
		{
			new VectorJsonConverter(),
			new QAngleJsonConverter()
		}
	};
	public static CCSGameRules GetGameRules()
	{
		var gameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault();

		if (gameRules == null)
		{
			throw new System.Exception("GameRules not found");
		}
			
		return gameRules.GameRules ?? throw new System.Exception("GameRules not found");
	}

	public static int GetPlayerCount(CsTeam? team)
	{
		var players = Utilities.GetPlayers();
		
		if (team == null)
		{
			return players.Count;
		}

		return players.Count(x => x.Team == team);
	}
	
	public static void CheckRoundDone()
	{
		var tHumanCount = GetCurrentNumPlayers(CsTeam.Terrorist);
		var ctHumanCount = GetCurrentNumPlayers(CsTeam.CounterTerrorist);

		if (tHumanCount == 0 || ctHumanCount == 0)
		{
			TerminateRound(RoundEndReason.TerroristsWin);
		}
	}
	
	public static void TerminateRound(RoundEndReason roundEndReason)
	{
		// TODO: once this stops crashing on windows use it there too
		if (Environment.OSVersion.Platform == PlatformID.Unix)
		{
			GetGameRules().TerminateRound(0.1f, roundEndReason);
		}
		else
		{
			Console.WriteLine(
				"[Executes] Windows server detected (Can't use TerminateRound) trying to kill all alive players instead.");
			var alivePlayers = Utilities.GetPlayers()
				.Where(IsValidPlayer)
				.Where(player => player.PawnIsAlive)
				.ToList();

			foreach (var player in alivePlayers)
			{
				player.CommitSuicide(false, true);
			}
		}
	}
	
	public static bool IsValidPlayer([NotNullWhen(true)] CCSPlayerController? player)
	{
		return player != null && player.IsValid;
	}

	public static bool IsPlayerConnected(CCSPlayerController player)
	{
		return player.Connected == PlayerConnectedState.PlayerConnected;
	}
	
	public static int GetCurrentNumPlayers(CsTeam? csTeam = null)
	{
		var players = 0;

		foreach (var player in Utilities.GetPlayers()
						.Where(player => IsValidPlayer(player) && IsPlayerConnected(player)))
		{
			if (csTeam == null || player.Team == csTeam)
			{
				players++;
			}
		}

		return players;
	}

	public static Dictionary<CsTeam, int> GetPlayerCountDict()
	{
		var players = Utilities.GetPlayers();

		return new Dictionary<CsTeam, int>()
		{
			{ CsTeam.CounterTerrorist, players.Count(x => x.Team == CsTeam.CounterTerrorist) },
			{ CsTeam.Terrorist, players.Count(x => x.Team == CsTeam.Terrorist) }
		};
	}

	public static int GetRandomInt(int min, int max)
	{
		return new Random().Next(min, max);
	}

	public static bool IsWarmup()
	{
		return GetGameRules()?.WarmupPeriod ?? false;
	}

	public static bool IsFreezeTime()
	{
		return GetGameRules()?.FreezePeriod ?? false;
	}
	
	public static void RestartGame()
	{
		if (!GetGameRules().WarmupPeriod)
		{
			CheckRoundDone();
		}

		Server.ExecuteCommand("mp_restartgame 1");
	}
	
	public static void GiveAndSwitchToBomb(CCSPlayerController player)
	{
		player.GiveNamedItem(CsItem.Bomb);
		NativeAPI.IssueClientCommand((int)player.UserId!, "slot5");
	}

	public static void RemoveHelmet(CCSPlayerController player)
	{
		if (player.PlayerPawn.Value == null || player.PlayerPawn.Value.ItemServices == null)
		{
			return;
		}

		var itemServices = new CCSPlayer_ItemServices(player.PlayerPawn.Value.ItemServices.Handle);
		itemServices.HasHelmet = false;
	}

	public static List<T> Shuffle<T>(IEnumerable<T> list)
	{
		var shuffledList = new List<T>(list); // Create a copy of the original list

		var n = shuffledList.Count;
		while (n > 1)
		{
			n--;
			var k = new Random().Next(n + 1);
			(shuffledList[n], shuffledList[k]) = (shuffledList[k], shuffledList[n]);
		}

		return shuffledList;
	}
	
	private const string ExecutesCfgDirectory = "/../../../../cfg/cs2-executes";
	private const string ExecutesCfgPath = $"{ExecutesCfgDirectory}/executes.cfg";
	
	public static void ExecuteExecutesConfiguration(string moduleDirectory)
	{
		if (!File.Exists(moduleDirectory + ExecutesCfgPath))
		{
			// make any directories required too
			Directory.CreateDirectory(moduleDirectory + ExecutesCfgDirectory);

			var retakesCfg = File.Create(moduleDirectory + ExecutesCfgPath);

			var retakesCfgContents = @"
				// Things you shouldn't change:
				bot_kick
				bot_quota 0
				mp_autoteambalance 0
				mp_do_warmup_period 1
				mp_forcecamera 1
				mp_give_player_c4 0
				mp_halftime 0
				mp_ignore_round_win_conditions 0
				mp_join_grace_time 0
				mp_match_can_clinch 0
				mp_maxmoney 0
				mp_playercashawards 0
				mp_respawn_on_death_ct 0
				mp_respawn_on_death_t 0
				mp_teamcashawards 0
				mp_warmup_pausetimer 0

				// Things you can change, and may want to:
				mp_autokick 0
				mp_c4timer 40
				mp_maxrounds 20
				mp_solid_teammates 1
				mp_friendlyfire 0

				mp_endmatch_votenextleveltime 3
				mp_endmatch_votenextmap 0
				mp_endmatch_votenextmap_keepcurrent 0

				sv_talk_enemy_dead 0
				sv_talk_enemy_living 0
				sv_deadtalk 1
				sv_allow_votes 0
				spec_replay_enable 0
				mp_round_restart_delay 5
				mp_freezetime 3

				// ESL deathcam settings
				spec_freeze_deathanim_time 0
				spec_freeze_panel_extended_time 0
				spec_freeze_time 2
				spec_freeze_time_lock 2

				echo [Executes] Config loaded!
			";

			var retakesCfgBytes = Encoding.UTF8.GetBytes(retakesCfgContents);
			retakesCfg.Write(retakesCfgBytes, 0, retakesCfgBytes.Length);

			retakesCfg.Close();
		}

		Server.ExecuteCommand("exec cs2-executes/executes.cfg");
	}
}