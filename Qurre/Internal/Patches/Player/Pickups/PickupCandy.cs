using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Usables.Scp330;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Player.Pickups
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup))]
    static class PickupCandy
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0); // ReferenceHub
            yield return new CodeInstruction(OpCodes.Ldarg_1); // Scp330Pickup
            yield return new CodeInstruction(OpCodes.Ldarg_2); // Scp330Bag
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PickupCandy), nameof(PickupCandy.Invoke)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        static bool Invoke(ReferenceHub ply, Scp330Pickup pickup, out Scp330Bag bag)
        {
            try
            {
                if (!Scp330Bag.TryGetBag(ply, out bag))
                {
                    ushort serial = (ushort)(pickup is null ? 0 : pickup.Info.Serial);
                    return ply.inventory.ServerAddItem(ItemType.SCP330, serial, pickup) is not null;
                }

                List<CandyKindID> list = new();

                if (pickup is null)
                    list.Add(Scp330Candies.GetRandom());
                else
                {
                    while (pickup.StoredCandies.Count > 0 && 6 > (bag.Candies.Count + list.Count))
                    {
                        list.Add(pickup.StoredCandies[0]);
                        pickup.StoredCandies.RemoveAt(0);
                    }
                }

                PickupCandyEvent ev = new(ply.GetPlayer(), bag, list);
                ev.InvokeEvent();

                if (!ev.Allowed)
                    ev.List.Clear();

                foreach (var candy in ev.List)
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
}