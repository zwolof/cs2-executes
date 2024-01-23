using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using ExecutesPlugin.Enums;
using ExecutesPlugin.Memory;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace ExecutesPlugin.Managers
{
    public sealed class GrenadeManager : BaseManager
    {
        public GrenadeManager() {}

        public void ThrowGrenade(CCSPlayerController player, EGrenade type, Vector position, QAngle angle, Vector velocity)
        {
            if(player.Pawn.Value == null)
            {
                return;
            }

            switch(type)
            {
                case EGrenade.Smoke:
                    SmokeFunctions.CSmokeGrenadeProjectile_CreateFunc.Invoke(
                        position.Handle,
                        angle.Handle,
                        velocity.Handle,
                        velocity.Handle,
                        player.Pawn.Value.Handle,
                        45,
                        player.TeamNum
                    );
                    break;
                
                default:
                    break;
            }

        }
    }
}