using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace ExecutesPlugin.Memory;

public static class GrenadeFunctions
{
	public static MemoryFunctionWithReturn<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, int, CSmokeGrenadeProjectile> CSmokeGrenadeProjectile_CreateFunc = new(
		Environment.OSVersion.Platform == PlatformID.Unix
			? @"55 4C 89 C1 48 89 E5 41 57 49 89 FF 41 56 45 89 CE"
			: @"48 8B C4 48 89 58 ? 48 89 68 ? 48 89 70 ? 57 41 56 41 57 48 81 EC ? ? ? ? 48 8B B4 24 ? ? ? ? 4D 8B F8"
	);
}