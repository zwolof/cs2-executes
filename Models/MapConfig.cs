using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;

namespace ExecutesPlugin.Models
{
	public class MapConfigScenario
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public EBombsite Bombsite { get; set; }
        public List<int>? Spawns { get; set; }
        public List<string>? Grenades { get; set; }
    }

	public class MapConfigSpawn
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public Vector? Position { get; set; }
        public QAngle? Angle { get; set; }
        public CsTeam? Team { get; set; }
        public ESpawnType Type { get; set; }
    }

	public class MapConfigGrenade
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
		public EGrenade Type { get; set; }
		
		[JsonConverter(typeof(VectorJsonConverter))]
        public Vector? Position { get; set; }
		
		[JsonConverter(typeof(QAngleJsonConverter))]
        public QAngle? Angle { get; set; }

		[JsonConverter(typeof(VectorJsonConverter))]
        public Vector? Velocity { get; set; }
        public float Delay { get; set; }
    }

	internal class VectorJsonConverter : JsonConverter<Vector>
	{
		public override Vector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.String)
			{
				throw new JsonException("Expected a string value.");
			}

			var stringValue = reader.GetString();
			if (stringValue == null)
			{
				throw new JsonException("String value is null.");
			}

			var values = stringValue.Split(' '); // Split by space

			if (values.Length != 3)
			{
				throw new JsonException("String value is not in the correct format (X Y Z).");
			}

			if (!float.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var x) ||
				!float.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var y) ||
				!float.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var z))
			{
				Console.WriteLine($"[Executes] Unable to parse Vector float values for: {stringValue}");
				throw new JsonException("Unable to parse Vector float values.");
			}

			return new Vector(x, y, z);
		}

		public override void Write(Utf8JsonWriter writer, Vector value, JsonSerializerOptions options)
		{
			// Convert Vector object to string representation (example assumes ToString() returns desired format)
			var vectorString = value.ToString();
			writer.WriteStringValue(vectorString);
		}
	}
	internal class QAngleJsonConverter : JsonConverter<QAngle>
	{
		public override QAngle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.String)
			{
				throw new JsonException("Expected a string value.");
			}

			var stringValue = reader.GetString();
			if (stringValue == null)
			{
				throw new JsonException("String value is null.");
			}

			var values = stringValue.Split(' '); // Split by space

			if (values.Length != 3)
			{
				throw new JsonException("String value is not in the correct format (X Y Z).");
			}

			if (!float.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var x) ||
				!float.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var y) ||
				!float.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var z))
			{
				Console.WriteLine($"[Executes] Unable to parse Vector float values for: {stringValue}");
				throw new JsonException("Unable to parse Vector float values.");
			}

			return new QAngle(x, y, z);
		}

		public override void Write(Utf8JsonWriter writer, QAngle value, JsonSerializerOptions options)
		{
			// Convert Vector object to string representation (example assumes ToString() returns desired format)
			var vectorString = value.ToString();
			writer.WriteStringValue(vectorString);
		}
	}

	public class MapConfig
    {
        public List<MapConfigScenario>? Scenarios { get; set; }
        public List<MapConfigSpawn>? Spawns { get; set; }
        public List<MapConfigGrenade>? Grenades { get; set; }
    }
}
