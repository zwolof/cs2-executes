using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;
using ExecutesPlugin.Models.JsonConverters;

namespace ExecutesPlugin.Models
{
	public class Grenade
	{
		public int? Id { get; set; }
		public string? Name { get; set; }
		public EGrenade Type { get; set; }
		
		[JsonConverter(typeof(VectorJsonConverter))]
		public Vector? Position { get; set; }
		
		[JsonConverter(typeof(QAngleJsonConverter))]
		public QAngle? Angle { get; set; }

		[JsonConverter(typeof(VectorJsonConverter))]
		public Vector? Velocity { get; set; }

		public CsTeam Team { get; set; }
		public float Delay { get; set; }
	}
}