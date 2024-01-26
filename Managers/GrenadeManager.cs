using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;
using ExecutesPlugin.Memory;
using ExecutesPlugin.Models;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace ExecutesPlugin.Managers
{
    public sealed class GrenadeManager : BaseManager
    {
        public GrenadeManager() {}

		public void SetupGrenades(Scenario scenario)
		{
			if(scenario.Grenades.Count == 0)
			{
				Console.WriteLine("[Executes] No grenades to setup.");
				return;
			}

			foreach(var grenade in scenario.Grenades[CsTeam.Terrorist])
			{
				ThrowGrenade(grenade);
			}
		}

        public void ThrowGrenade(Grenade grenade)
        {
            switch(grenade.Type)
            {
                case EGrenade.Smoke:
                    SmokeFunctions.CSmokeGrenadeProjectile_CreateFunc.Invoke(
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
			Console.WriteLine($"Threw grenade {grenade.Name}");
        }
    }
}