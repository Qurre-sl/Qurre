using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CustomPlayerEffects;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.EffectEvents;

[HarmonyPatch(typeof(StatusEffectBase), nameof(StatusEffectBase.ForceIntensity))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class ChangeIntensity
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        List<CodeInstruction> list = [..instructions];
        list.Last().labels.Add(retLabel);

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Call &&
                                          ins.operand is MethodBase methodBase &&
                                          methodBase.Name.Contains("ExecuteEvent")) + 3;

        if (index < 3)
        {
            Log.Error($"Creating Patch error: <Effect> [ChangeIntensity]: Index - {index} < 3");
            return list.AsEnumerable();
        }

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index]), // this [StatusEffectBase]

            new CodeInstruction(OpCodes.Ldarg_1), // value [byte]

            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ChangeIntensity), nameof(Invoke))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);

        return list.AsEnumerable();
    }

    private static bool Invoke(StatusEffectBase instance, byte value)
    {
        try
        {
            Player? pl = instance.Hub.GetPlayer();

            if (pl is null)
                return true;

            switch (instance._intensity)
            {
                case 0 when value > 0:
                {
                    EffectEnabledEvent ev = new(pl, instance);
                    ev.InvokeEvent();

                    return ev.Allowed;
                }
                case > 0 when value == 0:
                {
                    EffectDisabledEvent ev = new(pl, instance);
                    ev.InvokeEvent();

                    return ev.Allowed;
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Effect> [ChangeIntensity]: {e}\n{e.StackTrace}");
        }

        return true;
    }
}