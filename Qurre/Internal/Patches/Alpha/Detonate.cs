using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Alpha
{
    using Qurre.Events.Structs;

    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonate))]
    static class Detonate
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new(instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Ldarg_0);

            list[index].ExtractLabels();
            list.RemoveRange(0, index);

            list.InsertRange(0, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(AlphaDetonateEvent))[0]),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),
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