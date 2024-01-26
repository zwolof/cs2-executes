using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace ExecutesPlugin
{
	public static class Helpers
	{
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
		
		public static bool IsValidPlayer(CCSPlayerController? player)
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
	}
}