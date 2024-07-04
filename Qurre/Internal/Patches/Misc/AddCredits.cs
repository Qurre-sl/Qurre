using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Misc
{
    // don't remove pls
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.RefreshServerNameSafe))]
    static class AddCredits
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new(instructions);

            int index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                var ins = list[i];
                if (ins.opcode == OpCodes.Ret && list[i - 1].opcode == OpCodes.Ldloc_0)
                    index = i;
            }

            if (index < 1)
                return list.AsEnumerable();

            list.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldstr, $"<color=#00000000><size=1> Qurre v{API.Core.Version}</size></color>"),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(System.String), nameof(System.String.Concat), new[] { typeof(object), typeof(object) }))
            });

            return list.AsEnumerable();
        }
    }
}