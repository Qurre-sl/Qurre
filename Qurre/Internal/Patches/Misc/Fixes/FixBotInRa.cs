using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CentralAuth;
using HarmonyLib;
using Qurre.API;
using RemoteAdmin.Communication;

namespace Qurre.Internal.Patches.Misc.Fixes;

[HarmonyPatch(typeof(RaPlayerList), nameof(RaPlayerList.ReceiveData), typeof(CommandSender), typeof(string))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class FixBotInRa
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> list = [..instructions];

        int index = -1;
        int indexContinue = -1;

        bool started = false;
        for (int i = 0; i < list.Count; i++)
        {
            CodeInstruction? ins = list[i];

            if (started)
            {
                if (index < 0 && ins.opcode == OpCodes.Ldloc_S && ins.operand is LocalVariableInfo var &&
                    var.LocalType == typeof(ReferenceHub) && var.LocalIndex == 9)
                    index = i;
                if (indexContinue < 0 && ins.opcode == OpCodes.Ldloc_S &&
                    ins.operand is LocalVariableInfo { LocalIndex: 8 })
                    indexContinue = i;
            }
            else
            {
                if (ins.opcode == OpCodes.Stloc_S && ins.operand is LocalVariableInfo var &&
                    var.LocalType == typeof(ReferenceHub) && var.LocalIndex == 9)
                    started = true;
            }
        }

        if (index < 0)
        {
            Log.Error($"Creating Patch error: <Misc> {{Fixes}} [FixBotInRa]: Index - {index} < 0");
            return list.AsEnumerable();
        }

        if (indexContinue < 0)
        {
            Log.Error($"Creating Patch error: <Misc> {{Fixes}} [FixBotInRa]: indexContinue - {indexContinue} < 0");
            return list.AsEnumerable();
        }

        Label continueLabel = generator.DefineLabel();
        list[indexContinue].labels.Add(continueLabel);

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Ldloc_S, 9).MoveLabelsFrom(list[index]), // referenceHub
            new CodeInstruction(OpCodes.Ldfld,
                AccessTools.Field(typeof(ReferenceHub), nameof(ReferenceHub.authManager))),
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(PlayerAuthenticationManager),
                    nameof(PlayerAuthenticationManager.UserId))),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.IsNullOrEmpty))),
            new CodeInstruction(OpCodes.Brtrue, continueLabel)
        ]);

        return list.AsEnumerable();
    }
}
/* ....
 * if (string.IsNullOrEmpty(referenceHub.authManager.UserId))
 * {
 *    continue;
 * }
 * .....
 */