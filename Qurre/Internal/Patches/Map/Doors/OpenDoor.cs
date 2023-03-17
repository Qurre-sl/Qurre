using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using System;

namespace Qurre.Internal.Patches.Map.Doors
{
    [HarmonyPatch(typeof(DoorEventOpenerExtension), nameof(DoorEventOpenerExtension.Trigger))]
    static class OpenDoor
    {
        [HarmonyPrefix]
        static bool Call(DoorEventOpenerExtension __instance, DoorEventOpenerExtension.OpenerEventType eventType)
        {
            try
            {
                if (__instance is null || __instance.TargetDoor is null || __instance.TargetDoor.gameObject is null)
                    return true;

                Door door = __instance.TargetDoor.GetDoor();

                if (door is null)
                    return true;

                OpenDoorEvent ev = new(door, eventType);
                ev.InvokeEvent();

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Map> {{Doors}} [OpenDoor]: {e}\n{e.StackTrace}");
            }
            return true;
        }
    }
}