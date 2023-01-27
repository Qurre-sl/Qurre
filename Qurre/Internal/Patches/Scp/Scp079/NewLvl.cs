using HarmonyLib;
using PlayerRoles.PlayableScps.Scp079;
using System;

namespace Qurre.Internal.Patches.Scp.Scp079
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(Scp079TierManager), nameof(Scp079TierManager.AccessTierIndex), MethodType.Setter)]
    static class NewLvl
    {
        [HarmonyPrefix]
        static bool Call(Scp079TierManager __instance, ref int value)
        {
            try
            {
                if (__instance._accessTier == value)
                    return false;

                Scp079NewLvlEvent ev = new(__instance.Owner.GetPlayer(), value);
                ev.InvokeEvent();

                value = ev.Level;

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <SCPs> {{Scp079}} [NewLvl]: {e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}