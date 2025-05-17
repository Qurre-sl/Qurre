using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp079;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.ScpEvents.Scp079;

[HarmonyPatch(typeof(Scp079TierManager), nameof(Scp079TierManager.AccessTierIndex), MethodType.Setter)]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class NewLvl
{
    [HarmonyPrefix]
    private static bool Call(Scp079TierManager __instance, ref int value)
    {
        try
        {
            if (__instance._accessTier == value)
                return false;

            Player? pl = __instance.Owner.GetPlayer();

            if (pl is null)
                return false;

            Scp079NewLvlEvent ev = new(pl, value);
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