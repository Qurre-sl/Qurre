using System;
using HarmonyLib;
using InventorySystem;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Items
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.ServerSelectItem))]
    internal static class ChangeItem
    {
        [HarmonyPrefix]
        private static bool Call(Inventory __instance, ushort itemSerial)
        {
            try
            {
                if (itemSerial == __instance.CurItem.SerialNumber)
                {
                    return false;
                }

                ChangeItemEvent ev = new (
                    __instance._hub.GetPlayer(), Item.SafeGet(__instance.CurInstance),
                    itemSerial == 0 ? null : Item.Get(itemSerial));
                ev.InvokeEvent();

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Items}} [ChangeItem]: {e}\n{e.StackTrace}");
            }

            return true;
        }
    }
}