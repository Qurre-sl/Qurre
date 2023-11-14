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
    [HarmonyPatch(typeof(BreakableDoor), nameof(BreakableDoor.ServerDamage))]
    static class DamageDoor
    {
        [HarmonyPrefix]
        static bool Call(BreakableDoor __instance, ref float hp, DoorDamageType type)
        {
            try
            {
                Door door = __instance.GetDoor();

                if (door is null)
                    return true;

                DamageDoorEvent ev = new(door, type, hp);
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
}