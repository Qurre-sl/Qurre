using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MapGeneration;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.RoundEvents;

[HarmonyPatch(typeof(SeedSynchronizer), nameof(SeedSynchronizer.Update))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Waiting
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> list = [..instructions];

        int index = list.FindLastIndex(ins => ins.opcode == OpCodes.Callvirt &&
                                              ins.operand is MethodInfo { Name: nameof(Stopwatch.Restart) }) + 1;

        if (0 >= index)
            index = list.Count - 1;

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(WaitingEvent))[0]),
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent)))
        ]);

        return list.AsEnumerable();
    }
    /* ...
     * MapGenerated = true; // original instruction;
     * Loader.InvokeEvent(new WaitingEvent());
     * ...
     */
}