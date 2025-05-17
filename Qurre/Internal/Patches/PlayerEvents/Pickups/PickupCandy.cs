using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Usables.Scp330;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Pickups;

[HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class PickupCandy
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // ReferenceHub
        yield return new CodeInstruction(OpCodes.Ldarg_1); // Scp330Pickup
        yield return new CodeInstruction(OpCodes.Ldarg_2); // Scp330Bag
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PickupCandy), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static bool Invoke(ReferenceHub ply, Scp330Pickup pickup, out Scp330Bag? bag)
    {
        try
        {
            if (!Scp330Bag.TryGetBag(ply, out bag))
            {
                ushort serial = (ushort)(pickup == null ? 0 : pickup.Info.Serial);
                return ply.inventory.ServerAddItem(ItemType.SCP330, ItemAddReason.Scp914Upgrade, serial, pickup) is not
                    null;
            }

            List<CandyKindID> list = [];

            if (pickup == null)
                list.Add(Scp330Candies.GetRandom());
            else
                while (pickup.StoredCandies.Count > 0 && 6 > bag.Candies.Count + list.Count)
                {
                    list.Add(pickup.StoredCandies[0]);
                    pickup.StoredCandies.RemoveAt(0);
                }

            PickupCandyEvent ev = new(ply.GetPlayer() ?? throw new NullReferenceException(nameof(ply)), bag, list);
            ev.InvokeEvent();

            if (!ev.Allowed)
                ev.List.Clear();

            foreach (CandyKindID candy in ev.List)
                bag.TryAddSpecific(candy);

            if (bag.AcquisitionAlreadyReceived)
                bag.ServerRefreshBag();

            return ev.List.Count > 0;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Pickups}} [PickupCandy]: {e}\n{e.StackTrace}");
            bag = null;
            return false;
        }
    }
}