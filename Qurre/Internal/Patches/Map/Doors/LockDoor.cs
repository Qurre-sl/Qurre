using HarmonyLib;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using System;

namespace Qurre.Internal.Patches.Map.Doors
{
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerChangeLock))]
    static class LockDoor
    {
        [HarmonyPrefix]
        static bool Call(BreakableDoor __instance, DoorLockReason reason, ref bool newState)
        {
            try
            {
                Door door = __instance.GetDoor();

                if (door is null)
                    return true;

                LockDoorEvent ev = new(door, reason, newState);
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
}