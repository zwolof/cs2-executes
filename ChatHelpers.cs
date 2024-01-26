using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace ExecutesPlugin
{
	public static class ChatHelpers
	{
		public static readonly string ChatPrefix = $"[{ChatColors.Green}Executes{ChatColors.White}] ";

		public static void ChatMessage(CCSPlayerController player, string message)
		{
			player.PrintToChat($"{ChatPrefix} {message}");
		}

		public static void ChatMessageAll(string message, CsTeam? team = null)
		{
			var players = Utilities.GetPlayers();

			if (players.Count == 0)
			{
				Console.WriteLine($"No players found.");
				return;
			}

			foreach(var player in players)
			{
				if (!player.IsValidPlayer())
				{
					continue;
				}

				if (team != null && player.Team != team)
				{
					continue;
				}
				player.PrintToChat($"{ChatPrefix} {message}");
			}
		}
	}
}

