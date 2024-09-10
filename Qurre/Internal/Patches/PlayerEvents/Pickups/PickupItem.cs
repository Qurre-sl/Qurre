using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Pickups;

[HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class PickupItem
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        List<CodeInstruction> list = [..instructions];
        list.Last().labels.Add(retLabel);

        int delIndex = list.FindIndex(ins => ins.opcode == OpCodes.Call &&
                                             ins.operand is MethodBase methodBase &&
                                             methodBase.Name.Contains("ExecuteEvent")) + 3;

        if (delIndex < 3)
        {
            Log.Error($"Creating Patch error: <Player> {{Pickups}} [PickupItem]: Del Index - {delIndex} < 3");
            return list.AsEnumerable();
        }

        list.RemoveRange(0, delIndex);

        list[0].ExtractLabels();

        list.InsertRange(0,
        [
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PickupItem), nameof(Invoke))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);

        return list.AsEnumerable();
    }

    private static bool Invoke(ItemSearchCompletor instance)
    {
        try
        {
            Player? pl = instance.Hub.GetPlayer();
            Pickup? pickup = Pickup.SafeGet(instance.TargetPickup);

            if (pl is null || pickup is null)
                return false;

            PickupItemEvent ev = new(pl, pickup);
            ev.InvokeEvent();

            if (ev.Allowed)
                return true;

            PickupSyncInfo info = instance.TargetPickup.Info;
            info.InUse = false;
            info.Locked = false;
            instance.TargetPickup.NetworkInfo = info;

            return false;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Pickups}} [PickupItem]: {e}\n{e.StackTrace}");
            return true;
        }
    }
}