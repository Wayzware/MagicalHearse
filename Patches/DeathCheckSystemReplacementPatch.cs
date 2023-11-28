using Game.Simulation;

namespace Wayz.CS2.MagicalHearse;

[HarmonyPatch]
public static class MagicalHearseSystemInjector
{
    [HarmonyPatch(typeof(DeathCheckSystem), "OnCreate")]
    [HarmonyPrefix]
    public static bool OnCreate_Prefix(DeathCheckSystem __instance)
    {
        __instance.World.GetOrCreateSystemManaged<MagicalHearseSystem>();
        __instance.World.GetOrCreateSystemManaged<Game.UpdateSystem>().UpdateAt<MagicalHearseSystem>(Game.SystemUpdatePhase.GameSimulation);
        return true;
    }

    [HarmonyPatch(typeof(DeathCheckSystem), "OnUpdate")]
    [HarmonyPrefix]
    public static bool OnUpdate_Prefix(DeathCheckSystem __instance)
    {
        __instance.World.GetOrCreateSystemManaged<MagicalHearseSystem>().Update();
        return true;
    }
}

