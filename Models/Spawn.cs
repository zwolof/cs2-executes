using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;

namespace ExecutesPlugin.Models
{
	public class Spawn
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public CsTeam Team { get; set; }
		public Vector Position { get; set; }
		public QAngle Angle { get; set; }
		public ESpawnType SpawnType { get; set; }
		
		public Spawn(int id, string name, CsTeam team, Vector position, QAngle angle, ESpawnType spawnType)
		{
			Id = id;
			Name = name;
			Team = team;
			Position = position;
			Angle = angle;
			SpawnType = spawnType;
		}

		public ESpawnType GetSpawnFlags()
		{
			return SpawnType;
		}
	}
}