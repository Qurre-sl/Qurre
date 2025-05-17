using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using InventorySystem;
using Qurre.API;
using Qurre.API.Entities;
using Qurre.API.Entities.Items;
using Qurre.API.Utils.Entities;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Items;

[HarmonyPatch(typeof(Inventory), nameof(Inventory.ServerSelectItem))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class ChangeItem
{
    [HarmonyPrefix]
    private static bool Call(Inventory __instance, ushort itemSerial)
    {
        try
        {
            if (itemSerial == __instance.CurItem.SerialNumber)
                return false;

            var player = __instance._hub.GetPlayer();
            if (player is null) return false;

            if (!EntityManager.TryGet(__instance.CurInstance, out IItem? oldItem)) return true;
            var newItem = itemSerial == 0 ? null : ItemsHelper.GetItemBySerial(itemSerial);

            ChangeItemEvent ev = new(player, oldItem, newItem);
            ev.InvokeEvent();

            return ev.IsAllowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Items}} [ChangeItem]: {e}\n{e.StackTrace}");
        }

        return true;
    }
}