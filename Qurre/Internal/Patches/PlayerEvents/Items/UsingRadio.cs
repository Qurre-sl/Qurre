using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items;
using InventorySystem.Items.Radio;
using Qurre.API;
using Qurre.Events.Structs;
using UnityEngine;

namespace Qurre.Internal.Patches.PlayerEvents.Items;

[HarmonyPatch(typeof(RadioItem), nameof(RadioItem.Update))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class UsingRadio
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        LocalBuilder @event = generator.DeclareLocal(typeof(UsingRadioEvent));

        List<CodeInstruction> list = [.. instructions];
        list.Last().labels.Add(retLabel);

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Stloc_0);

        if (index < 0)
        {
            Log.Error($"Creating Patch error: <Player> {{Items}} [UsingRadio]: Index - {index} < 0");
            return list.AsEnumerable();
        }

        list.InsertRange(index + 1,
        [
            new CodeInstruction(OpCodes.Ldarg_0), // this {base}
            new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner))),
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Ldarg_0), // this

            //new(OpCodes.Call, AccessTools.PropertyGetter(typeof(UnityEngine.Time), nameof(UnityEngine.Time.deltaTime))),
            new CodeInstruction(OpCodes.Ldloc_0), // num [float]
            //new(OpCodes.Ldc_R4, 100),
            //new(OpCodes.Div), // num / 100 { = _div }
            //new(OpCodes.Mul), // Time * _div

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UsingRadioEvent))[0]),
            new CodeInstruction(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            // if(!@event.Allowed) return;
            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(UsingRadioEvent), nameof(UsingRadioEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);

        bool found = false;
        int startIndex = -1;
        int endIndex = -1;
        for (int i = index; i < list.Count; i++)
        {
            CodeInstruction? ins = list[i];

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (!found && ins.opcode == OpCodes.Call &&
                ins.operand is MethodBase { Name: nameof(EventsManager.Loader.InvokeEvent) })
                found = true;
            else if (found && startIndex < 0 && ins.opcode == OpCodes.Callvirt && list[i + 1].opcode == OpCodes.Brfalse)
                startIndex = i + 2;
            else if (startIndex > 0 && ins.opcode == OpCodes.Call &&
                     ins.operand is MethodBase { Name: nameof(Mathf.Clamp01) })
                endIndex = i + 2;
        }

        if (startIndex < 0)
        {
            Log.Error($"Creating Patch error: <Player> {{Items}} [UsingRadio]: Start Index - {startIndex} < 0");
            return list.AsEnumerable();
        }

        if (endIndex < 0)
        {
            Log.Error($"Creating Patch error: <Player> {{Items}} [UsingRadio]: End Index - {endIndex} < 0");
            return list.AsEnumerable();
        }

        list.RemoveRange(startIndex, endIndex - startIndex);

        list.InsertRange(startIndex,
        [
            new CodeInstruction(OpCodes.Ldarg_0), // this

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UsingRadio), nameof(GetBattery))),

            new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(RadioItem), nameof(RadioItem._battery)))
        ]);

        return list.AsEnumerable();
    }

    private static float GetBattery(UsingRadioEvent @event)
    {
        return Mathf.Clamp01(@event.Battery / 100 - @event.Consumption / 100);
    }
}