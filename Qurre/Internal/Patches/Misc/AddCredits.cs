using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Qurre.API;

namespace Qurre.Internal.Patches.Misc
{
    // don't remove plz
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.RefreshServerNameSafe))]
    internal static class AddCredits
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new (instructions);

            int index = -1;

            for (var i = 0; i < list.Count; i++)
            {
                CodeInstruction ins = list[i];

                if (ins.opcode == OpCodes.Ret && list[i - 1].opcode == OpCodes.Ldloc_0)
                {
                    index = i;
                }
            }

            if (index < 1)
            {
                return list.AsEnumerable();
            }

            list.InsertRange(
                index, new[]
                {
                    new (OpCodes.Ldstr, $"<color=#00000000><size=1> Qurre v{Core.Version}-beta</size></color>"),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), new[] { typeof(object), typeof(object) }))
                });

            return list.AsEnumerable();
        }
    }
}