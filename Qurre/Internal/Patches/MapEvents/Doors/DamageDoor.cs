using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.MapEvents.Doors;

[HarmonyPatch(typeof(BreakableDoor), nameof(BreakableDoor.ServerDamage))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class DamageDoor
{
    [HarmonyPrefix]
    private static bool Call(BreakableDoor __instance, ref float hp, DoorDamageType type)
    {
        try
        {
            if (__instance == null)
                return true;

            DamageDoorEvent ev = new(__instance.GetDoor(), type, hp);
            ev.InvokeEvent();

            hp = ev.Damage;
            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Map> {{Doors}} [DamageDoor]: {e}\n{e.StackTrace}");
        }

        return true;
    }
}