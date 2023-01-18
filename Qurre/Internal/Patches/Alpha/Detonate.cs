using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.Alpha
{
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonate))]
    internal static class Detonate
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new (instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Ldarg_0);

            list[index].ExtractLabels();
            list.RemoveRange(0, index);

            list.InsertRange(
                0, new[]
                {
                    new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(AlphaDetonateEvent))[0]),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent)))
                });

            return list.AsEnumerable();
        }
        /*
         * < Remove nwapi event, because "if(!allowed) return;" doesn't work, bruh >
         * new AlphaDetonateEvent().InvokeEvent();
         * ...
        */
    }
}