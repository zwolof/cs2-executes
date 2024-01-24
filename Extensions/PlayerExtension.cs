using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Models;

namespace ExecutesPlugin.Extensions
{
	public static class PlayerExtension
	{
		public static void ChatMessage(this CCSPlayerController player, string message)
		{
			player.PrintToChat($"[Executes] {message}");
		}

		public static bool IsValidPlayer(this CCSPlayerController? player)
		{
			return player != null && player.IsValid && player.Connected == PlayerConnectedState.PlayerConnected;
		}

		public static void MoveToSpawn(this CCSPlayerController player, Spawn spawn)
		{
			player.Teleport(spawn.Position, spawn.Angle, new Vector(IntPtr.Zero));
		}
	}
}