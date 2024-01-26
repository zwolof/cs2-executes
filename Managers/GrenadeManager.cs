using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;
using ExecutesPlugin.Memory;
using ExecutesPlugin.Models;
using TimerFlags = CounterStrikeSharp.API.Modules.Timers.TimerFlags;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace ExecutesPlugin.Managers
{
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

			foreach(var grenade in scenario.Grenades[CsTeam.Terrorist])
			{
				var nadeThrowPercentage = new Random().Next(0, 100);

				if(nadeThrowPercentage < 66)
				{
					Console.WriteLine($"[Executes] Skipping \"{grenade.Name}\".");
					continue;
				}
				// Server.PrintToChatAll($"[Executes] Throwing {grenade.Name} with a delay of {grenade.Delay}");
				_ = new Timer(freezeTimeDuration + grenade.Delay, () => ThrowGrenade(grenade), TimerFlags.STOP_ON_MAPCHANGE);
			}
		}

        public void ThrowGrenade(Grenade grenade)
        {
            switch(grenade.Type)
            {
                case EGrenade.Smoke:
                    GrenadeFunctions.CSmokeGrenadeProjectile_CreateFunc.Invoke(
                        grenade.Position!.Handle,
                        grenade.Angle!.Handle,
                        grenade.Velocity!.Handle,
                        grenade.Velocity.Handle,
                        IntPtr.Zero,
                        45,
                        (int)grenade.Team
                    );
                    break;
                
                default:
                    break;
            }
			Console.WriteLine($"[Executes] Threw grenade {grenade.Name}");
        }
    }
}