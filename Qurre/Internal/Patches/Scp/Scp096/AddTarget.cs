namespace Qurre.Internal.Patches.Scp.Scp096;

using HarmonyLib;
using PlayerRoles.PlayableScps.Scp096;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

[HarmonyPatch(typeof(Scp096TargetsTracker), nameof(Scp096TargetsTracker.AddTarget))]
static class AddTarget
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        foreach (CodeInstruction ins in instructions)
        {
            if (ins.opcode == OpCodes.Newobj && ins.operand is not null && ins.operand is MethodBase methodBase1 &&
                methodBase1.DeclaringType.FullName.Contains("PluginAPI") && methodBase1.DeclaringType.FullName.Contains("Events"))
            {
                yield return new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(Scp096AddTargetEvent))[0]);
                continue;
            }

            if (ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase2 &&
                methodBase2.Name.Contains("ExecuteEvent"))
            {
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AddTarget), nameof(AddTarget.Invoke)));
                continue;
            }

            yield return ins;
        }
    }

    static bool Invoke(Scp096AddTargetEvent @event)
    {
        @event.InvokeEvent();
        return @event.Allowed;
    }
}