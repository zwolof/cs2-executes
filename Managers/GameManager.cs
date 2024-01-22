using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Executes.Enums;
using CS2Executes.Memory;
using CS2Executes.Models;
using Newtonsoft.Json;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace CS2Executes.Managers
{
	public sealed class GameManager : BaseManager
	{
		private List<Execute> _executes = new();

		public GameManager() { }

		public bool LoadSpawns(string map)
		{
			var configExists = File.Exists($"{map}.json");

			if(!configExists)
			{
				Console.WriteLine($"[Executes] {map}.json does not exist.");
				return false;
			}

			var config = File.ReadAllText("executes.json");

			var parsedConfig = JsonConvert.DeserializeObject<List<Execute>>(config);

			if(parsedConfig == null)
			{
				Console.WriteLine($"[Executes] Failed to parse {map}.json");
				return false;
			}

			_executes = parsedConfig;

			Console.WriteLine($"-------------------------- Loaded {_executes.Count} executes.");
			return true;
		}

		public void SetupScenario()
		{
			
		}
	}
}