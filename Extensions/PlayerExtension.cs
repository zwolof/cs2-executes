using CounterStrikeSharp.API.Core;

namespace CS2Executes
{
	public static class PlayerExtension
	{
		public static void PrintToChat(this CCSPlayerController player, string message)
		{
			player.PrintToChat($"[Prefix] {message}");
		}
	}
}