using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Executes.Enums;

namespace CS2Executes.Models
{
	public class Grenades
	{
		
	}
	public class Spawn
	{
		public string Name { get; set; }
		public CsTeam Team { get; set; }
		public Vector Position { get; set; }
		public QAngle Angle { get; set; }

		public Spawn(string name, CsTeam team, Vector position, QAngle angle)
		{
			Name = name;
			Team = team;
			Position = position;
			Angle = angle;
		}

		public void SpawnPlayer(CCSPlayerController player)
		{
			player.Teleport(Position, Angle, new Vector(IntPtr.Zero));
		}
	}
}