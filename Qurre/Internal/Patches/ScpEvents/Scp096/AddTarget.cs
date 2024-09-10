using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp096;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.ScpEvents.Scp096;

[HarmonyPatch(typeof(Scp096TargetsTracker), nameof(Scp096TargetsTracker.AddTarget))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class AddTarget
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction ins in instructions)
        {
            if (ins.opcode == OpCodes.Newobj && ins.operand is MethodBase
                {
                    DeclaringType.FullName: not null
                } methodBase1 &&
                methodBase1.DeclaringType.FullName.Contains("PluginAPI") &&
                methodBase1.DeclaringType.FullName.Contains("Events"))
            {
                yield return new CodeInstruction(OpCodes.Newobj,
                    AccessTools.GetDeclaredConstructors(typeof(Scp096AddTargetEvent))[0]);
                continue;
            }

            if (ins.opcode == OpCodes.Call && ins.operand is MethodBase methodBase2 &&
                methodBase2.Name.Contains("ExecuteEvent"))
            {
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AddTarget), nameof(Invoke)));
                continue;
            }

            yield return ins;
        }
    }

    private static bool Invoke(Scp096AddTargetEvent @event)
    {
        @event.InvokeEvent();
        return @event.Allowed;
    }
}