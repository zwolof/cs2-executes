using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2Executes
{
	public class Helpers
	{
		public static CCSGameRules? GetGameRules()
		{
			return Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault()!.GameRules;
		}

		public static int GetPlayerCount(CsTeam? team)
		{
			var players = Utilities.GetPlayers();
			
			if(team == null)
			{
				return players.Count;
			}

			return players.Count(x => x.Team == team);
		}
	}
}