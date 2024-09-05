using HarmonyLib;
using InventorySystem.Items.Firearms.Attachments;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Interact
{
    [HarmonyPatch(typeof(WorkstationController), nameof(WorkstationController.ServerInteract))]
    static class InteractWorkStation
    {
        [HarmonyPrefix]
        static bool Call(WorkstationController __instance, ReferenceHub ply, byte colliderId)
        {
            InteractWorkStationEvent ev = new(ply.GetPlayer(), __instance.GetWorkStation(), colliderId);

            if (ev.Station is null)
                return true;

            ev.InvokeEvent();

            return ev.Allowed;
        }
    }
}