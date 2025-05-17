using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.MapEvents.Doors;

[HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerChangeLock))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class LockDoor
{
    [HarmonyPrefix]
    private static bool Call(BreakableDoor __instance, DoorLockReason reason, ref bool newState)
    {
        try
        {
            if (__instance == null)
                return true;

            LockDoorEvent ev = new(__instance.GetDoor(), reason, newState);
            ev.InvokeEvent();

            newState = ev.NewState;
            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Map> {{Doors}} [LockDoor]: {e}\n{e.StackTrace}");
        }

        return true;
    }
}