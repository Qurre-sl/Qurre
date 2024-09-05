using HarmonyLib;
using InventorySystem.Items.Pickups;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using Scp914;
using System;
using UnityEngine;

namespace Qurre.Internal.Patches.Scp.Scp914
{
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.ProcessPickup))]
    static class UpgradePickup
    {
        [HarmonyPrefix]
        static bool Call(ItemPickupBase pickup, ref bool upgradeDropped, ref Vector3 moveVector, ref Scp914KnobSetting setting)
        {
            try
            {
                Scp914UpgradePickupEvent ev = new(pickup, upgradeDropped, moveVector, setting);
                ev.InvokeEvent();

                if (!ev.Allowed)
                    return false;

                upgradeDropped = ev.UpgradeDropped;
                moveVector = ev.Move;
                setting = ev.Setting;

                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <SCPs> {{Scp914}} [UpgradePickup]: {e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}