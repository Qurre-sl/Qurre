using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Pickups;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Pickups
{
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropItem))]
    static class DroppedItem
    {
        [HarmonyPostfix]
        static void Call(Inventory inv, ItemPickupBase __result) =>
            new DroppedItemEvent(inv._hub.GetPlayer(), Pickup.SafeGet(__result)).InvokeEvent();
    }
}