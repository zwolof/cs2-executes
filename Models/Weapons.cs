using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;

namespace ExecutesPlugin.Models
{
	public class Weapons
	{
		public string? DesignerName { get; set; }
		public int Ammo { get; set; }
		public int ReserveAmmo { get; set; }
		public Vector Velocity { get; set; }
		public float Delay { get; set; }
	}
}