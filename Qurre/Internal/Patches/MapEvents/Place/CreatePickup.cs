using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.MapEvents.Place;

[HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup), typeof(Inventory),
    typeof(ItemBase), typeof(PickupSyncInfo), typeof(bool), typeof(Action<ItemPickupBase>))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class CreatePickup
{
    [HarmonyPrefix]
    private static void Call(Inventory inv, PickupSyncInfo psi, ref bool spawn)
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