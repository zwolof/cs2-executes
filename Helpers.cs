using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace ExecutesPlugin
{
	public class Helpers
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