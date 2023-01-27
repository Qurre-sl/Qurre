using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Pickups;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using System;

namespace Qurre.Internal.Patches.Map.Place
{
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup))]
    static class CreatePickup
    {
        [HarmonyPrefix]
        static void Call(Inventory inv, PickupSyncInfo psi, ref bool spawn)
        {
            try
            {
                CreatePickupEvent ev = new(psi, inv);
                ev.InvokeEvent();

                spawn = ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Map> {{Place}} [CreatePickup]: {e}\n{e.StackTrace}");
            }
        }
    }
}