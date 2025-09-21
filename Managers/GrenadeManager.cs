using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;
using ExecutesPlugin.Memory;
using ExecutesPlugin.Models;
using TimerFlags = CounterStrikeSharp.API.Modules.Timers.TimerFlags;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace ExecutesPlugin.Managers;

public sealed class GrenadeManager : BaseManager
{
	public GrenadeManager() {}

	public void SetupGrenades(Scenario scenario)
	{
		if (scenario.Grenades.Count == 0)
		{
			Console.WriteLine("[Executes] No grenades to setup.");
			return;
		}

		var freezeTimeDuration = 0;
		var freezeTime = ConVar.Find("mp_freezetime");
		if (freezeTime != null)
		{
			freezeTimeDuration = freezeTime.GetPrimitiveValue<int>();
		}
		else
		{
			Console.WriteLine("[Executes] mp_freezetime not found.");
		}

		var teams = new List<CsTeam>
		{ 
			CsTeam.Terrorist, 
			CsTeam.CounterTerrorist 
		};
		var nadesThrown = new Dictionary<CsTeam, int>
		{
			{ CsTeam.Terrorist, 0 },
			{ CsTeam.CounterTerrorist, 0 },
		};

		foreach(var team in teams)
		{
			foreach(var grenade in scenario.Grenades[team])
			{
				var nadeThrowPercentage = new Random().Next(0, 100);

				if (nadesThrown[team] >= Helpers.GetPlayerCount(team))
				{
					Console.WriteLine($"[Executes] Skipping \"{grenade.Name}\".");
					continue;
				}

				new Timer(freezeTimeDuration + grenade.Delay, () => ThrowGrenade(grenade), TimerFlags.STOP_ON_MAPCHANGE);
				nadesThrown[team] += 1;
			}
		}
	}

	public void ThrowGrenade(Grenade grenade)
	{
		CBaseCSGrenadeProjectile? createdGrenade = null;
		switch (grenade.Type)
		{
			case EGrenade.Smoke:
			{
				createdGrenade = GrenadeFunctions.CSmokeGrenadeProjectile_CreateFunc.Invoke(
					grenade.Position!.Handle,
					grenade.Angle!.Handle,
					grenade.Velocity!.Handle,
					grenade.Velocity.Handle,
					IntPtr.Zero,
					45,
					(int)grenade.Team);
				break;
			}
			case EGrenade.Molotov:
			case EGrenade.Incendiary:
			{
				createdGrenade = GrenadeFunctions.CMolotovProjectile_CreateFunc.Invoke(
					grenade.Position!.Handle,
					grenade.Angle!.Handle,
					grenade.Velocity!.Handle,
					grenade.Velocity.Handle,
					IntPtr.Zero,
					grenade.Type == EGrenade.Molotov ? 46 : 48);
				break;
			}
			case EGrenade.HighExplosive:
			{
				createdGrenade = GrenadeFunctions.CHEGrenadeProjectile_CreateFunc.Invoke(
					grenade.Position!.Handle,
					grenade.Angle!.Handle,
					grenade.Velocity!.Handle,
					grenade.Velocity.Handle,
					IntPtr.Zero,
					44);
				break;
			}
			case EGrenade.Decoy:
			{
				createdGrenade = GrenadeFunctions.CDecoyProjectile_CreateFunc.Invoke(
					grenade.Position!.Handle,
					grenade.Angle!.Handle,
					grenade.Velocity!.Handle,
					grenade.Velocity.Handle,
					IntPtr.Zero,
					47);
				break;
			}
			case EGrenade.Flashbang:
			{
				createdGrenade = Utilities.CreateEntityByName<CFlashbangProjectile>("flashbang_projectile");
				if (createdGrenade == null)
				{
					return;
				}

				createdGrenade.DispatchSpawn();
				break;
			}
			default:
				Console.WriteLine($"[Executes] Unimplemented Grenade type {grenade.Type}");
				break;
		}

		if (createdGrenade != null && createdGrenade.DesignerName != "smokegrenade_projectile")
		{
			createdGrenade.Teleport(grenade.Position!, grenade.Angle!, grenade.Velocity!);

			createdGrenade.InitialPosition.X = grenade.Position!.X;
			createdGrenade.InitialPosition.Y = grenade.Position!.Y;
			createdGrenade.InitialPosition.Z = grenade.Position!.Z;

			createdGrenade.InitialVelocity.X = grenade.Velocity!.X;
			createdGrenade.InitialVelocity.Y = grenade.Velocity!.Y;
			createdGrenade.InitialVelocity.Z = grenade.Velocity!.Z;

			createdGrenade.AngVelocity.X = grenade.Velocity!.X;
			createdGrenade.AngVelocity.Y = grenade.Velocity!.Y;
			createdGrenade.AngVelocity.Z = grenade.Velocity!.Z;

			createdGrenade.TeamNum = (byte)grenade.Team;
		}

		Console.WriteLine($"[Executes] Threw grenade {grenade.Name}");
	}
}