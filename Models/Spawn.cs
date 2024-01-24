using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;

namespace ExecutesPlugin.Models
{
	public class Spawn
	{
		public int? Id { get; set; }
		public string? Name { get; set; }
		public Vector? Position { get; set; }
		public QAngle? Angle { get; set; }
		public CsTeam? Team { get; set; }
		public ESpawnType Type { get; set; }
	}
}