using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;

namespace ExecutesPlugin.Models
{
	public class Grenade
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public EGrenade Type { get; set; }
		public Vector? Position { get; set; }
		public QAngle? Angle { get; set; }
		public Vector? Velocity { get; set; }
		public float Delay { get; set; }
	}
}