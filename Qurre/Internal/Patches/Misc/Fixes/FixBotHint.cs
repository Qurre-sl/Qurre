using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using CentralAuth;
using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;
using Qurre.API;

namespace Qurre.Internal.Patches.Misc.Fixes;

[HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.ShowHitIndicator))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class FixBotHint
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        List<CodeInstruction> list = [..instructions];
        list.Last().labels.Add(retLabel);

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Ldloc_0);

        if (index < 0)
        {
            Log.Error($"Creating Patch error: <Misc> {{Fixes}} [FixBotHint]: Index - {index} < 0");
            return list.AsEnumerable();
        }

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(list[index]), // referenceHub
            new CodeInstruction(OpCodes.Ldfld,
                AccessTools.Field(typeof(ReferenceHub), nameof(ReferenceHub.authManager))),
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(PlayerAuthenticationManager),
                    nameof(PlayerAuthenticationManager.UserId))),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.IsNullOrEmpty))),
            new CodeInstruction(OpCodes.Brtrue, retLabel)
        ]);

        return list.AsEnumerable();
    }
}
/* ....
 * if (string.IsNullOrEmpty(referenceHub.authManager.UserId))
 * {
 *    return;
 * }
 * .....
 */