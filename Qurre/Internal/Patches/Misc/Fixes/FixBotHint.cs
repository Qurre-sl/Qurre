using CentralAuth;
using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;
using Qurre.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Misc.Fixes
{
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.ShowHitIndicator))]
    static class FixBotHint
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            List<CodeInstruction> list = new(instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Ldloc_0);

            if (index < 0)
            {
                Log.Error($"Creating Patch error: <Misc> {{Fixes}} [FixBotHint]: Index - {index} < 0");
                return list.AsEnumerable();
            }

            list.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(list[index]), // referenceHub
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ReferenceHub), nameof(ReferenceHub.authManager))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.UserId))),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(String), nameof(String.IsNullOrEmpty))),
                new CodeInstruction(OpCodes.Brtrue, retLabel),
            });

            return list.AsEnumerable();
        }
        /* ....
         * if (string.IsNullOrEmpty(referenceHub.authManager.UserId))
         * {
         *    return;
         * }
         * .....
         */
    }
}