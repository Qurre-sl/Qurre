using HarmonyLib;
using InventorySystem.Items.Firearms.Attachments;
using Mirror;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Map.Objects
{
    [HarmonyPatch(typeof(WorkstationController), nameof(WorkstationController.Update))]
    static class WorkStationUpdate
    {
        [HarmonyPrefix]
        static bool Call(WorkstationController __instance)
        {
            if (!NetworkServer.active || __instance.Status == 0)
            {
                return false;
            }

            WorkStationUpdateEvent ev = new(__instance.GetWorkStation());
            ev.InvokeEvent();

            return ev.Allowed;
        }
    }
}