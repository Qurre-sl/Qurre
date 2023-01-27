using CustomPlayerEffects;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Effect
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(StatusEffectBase), nameof(StatusEffectBase.Intensity), MethodType.Setter)]
    static class ChangeIntensity
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            List<CodeInstruction> list = new(instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name.Contains("ExecuteEvent")) + 3;

            if (index < 3)
            {
                Log.Error($"Creating Patch error: <Effect> [ChangeIntensity]: Index - {index} < 3");
                return list.AsEnumerable();
            }

            list.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index]), // this [StatusEffectBase]

                new CodeInstruction(OpCodes.Ldarg_1), // value [byte]

                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ChangeIntensity), nameof(ChangeIntensity.Invoke))),
                new CodeInstruction(OpCodes.Brfalse, retLabel),
            });

            return list.AsEnumerable();
        }

        static bool Invoke(StatusEffectBase instance, byte value)
        {
            try
            {
                var pl = instance.Hub.GetPlayer();

                if (pl is null)
                    return true;

                if (instance._intensity == 0 && value > 0)
                {
                    EffectEnabledEvent ev = new(pl, instance);
                    ev.InvokeEvent();

                    return ev.Allowed;
                }
                else if (instance._intensity > 0 && value == 0)
                {
                    EffectDisabledEvent ev = new(pl, instance);
                    ev.InvokeEvent();

                    return ev.Allowed;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Effect> [ChangeIntensity]: {e}\n{e.StackTrace}");
            }

            return true;
        }
    }
}