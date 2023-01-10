using HarmonyLib;
using PlayerRoles.PlayableScps.Scp173;
using PostProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static HarmonyLib.Code;

namespace Qurre.Internal.Patches.Scp.Scp173
{
    [HarmonyPatch(typeof(Scp173BlinkTimer), nameof(Scp173BlinkTimer.ServerBlink))]
    static class Blink
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new (instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Stloc_0);
        }
    }
}
