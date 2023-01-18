using System;
using HarmonyLib;
using InventorySystem;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Pickups
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropAmmo))]
    internal static class DropAmmo
    {
        [HarmonyPrefix]
        private static bool Call(Inventory __instance, ref byte ammoType, ref ushort amount)
        {
            try
            {
                DropAmmoEvent ev = new (__instance._hub.GetPlayer(), ((ItemType)ammoType).GetAmmoType(), amount);
                ev.InvokeEvent();

                ammoType = (byte)ev.Type.GetItemType();
                amount = ev.Amount;

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Pickups}} [DropAmmo]: {e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}