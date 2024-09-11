using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Pickups;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Pickups;

[HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropItem))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class DroppedItem
{
    [HarmonyPostfix]
    private static void Call(Inventory inv, ItemPickupBase __result)
    {
        Player? pl = inv._hub.GetPlayer();
        Pickup? pickup = Pickup.SafeGet(__result);

        if (pl is null || pickup is null)
            return;

        new DroppedItemEvent(pl, pickup).InvokeEvent();
    }
}