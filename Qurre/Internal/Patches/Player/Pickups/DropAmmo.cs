using HarmonyLib;
using InventorySystem;
using System;

namespace Qurre.Internal.Patches.Player.Pickups
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropAmmo))]
    static class DropAmmo
    {
        [HarmonyPrefix]
        static bool Call(Inventory inv, ref ItemType ammoType, ref ushort amount)
        {
            try
            {
                DropAmmoEvent ev = new(inv._hub.GetPlayer(), ammoType.GetAmmoType(), amount);
                ev.InvokeEvent();

                ammoType = ev.Type.GetItemType();
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