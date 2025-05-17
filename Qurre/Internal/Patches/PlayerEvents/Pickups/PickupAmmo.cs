using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Searching;
using Qurre.API;
using Qurre.API.Entities.Characters;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Pickups;

[HarmonyPatch(typeof(AmmoSearchCompletor), nameof(AmmoSearchCompletor.Complete))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class PickupAmmo
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        List<CodeInstruction> list = [.. instructions];
        list.Last().labels.Add(retLabel);

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Call &&
                                          ins.operand is MethodBase methodBase &&
                                          methodBase.Name.Contains("ExecuteEvent")) + 3;

        if (index < 3)
        {
            Log.Error($"Creating Patch error: <Player> {{Pickups}} [PickupAmmo]: Index - {index} < 3");
            return list.AsEnumerable();
        }

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index]),

            new CodeInstruction(OpCodes.Ldloc_0),

            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PickupAmmo), nameof(Invoke))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);

        return list.AsEnumerable();
    }

    private static bool Invoke(AmmoSearchCompletor instance, AmmoPickup ammo)
    {
        try
        {
            if (!Player.TryGet(instance.Hub, out var player))
                return true;

            PickupAmmoEvent ev = new(player, instance.TargetPickup, ammo);
            ev.InvokeEvent();

            if (ev.IsAllowed)
                return true;

            var syncInfo = instance.TargetPickup.Info;
            syncInfo.InUse = false;
            syncInfo.Locked = false;
            instance.TargetPickup.NetworkInfo = syncInfo;

            return false;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Pickups}} [PickupAmmo]: {e}\n{e.StackTrace}");
            return true;
        }
    }
}