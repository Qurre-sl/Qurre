using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using InventorySystem.Items.Pickups;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using Scp914;

namespace Qurre.Internal.Patches.ScpEvents.Scp914;

[HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.ProcessPickup))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class UpgradePickup
{
    [HarmonyPrefix]
    private static bool Call(ItemPickupBase pickup, ref bool upgradeDropped,
        ref Scp914KnobSetting setting)
    {
        try
        {
            Scp914UpgradePickupEvent ev = new(pickup, upgradeDropped, API.World.Scp914.MoveVector, setting);
            ev.InvokeEvent();

            if (!ev.Allowed)
                return false;

            upgradeDropped = ev.UpgradeDropped;
            // moveVector = ev.Move;
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