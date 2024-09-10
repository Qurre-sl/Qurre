using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp173;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.ScpEvents.Scp173;

[HarmonyPatch(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.CheckRemovedPlayer))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class RemovedObserver1
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> list = [..instructions];

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Call &&
                                          ins.operand is MethodBase methodBase &&
                                          methodBase.Name.Contains("set_CurrentObservers")) + 1;

        if (index < 1)
        {
            Log.Error($"Creating Patch error: <SCPs> {{Scp173}} [RemovedObserver #1]: Index - {index} < 1");
            return list.AsEnumerable();
        }

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(list[index]), // target [ReferenceHub]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Ldarg_0), // instance [Scp173ObserversTracker]
            new CodeInstruction(OpCodes.Call,
                AccessTools.PropertyGetter(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.Owner))),
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Newobj,
                AccessTools.GetDeclaredConstructors(
                    typeof(Scp173RemovedObserverEvent))[0]), // new Scp173RemovedObserverEvent(...);
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader),
                    nameof(EventsManager.Loader.InvokeEvent))) // .InvokeEvent();
        ]);

        return list.AsEnumerable();
    }
}

[HarmonyPatch(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.UpdateObserver))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class RemovedObserver2
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction ins in instructions)
        {
            yield return ins;

            if (ins.opcode != OpCodes.Callvirt || ins.operand is not MethodBase methodBase ||
                !methodBase.Name.Contains("Remove")) continue;

            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)]));

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call,
                AccessTools.PropertyGetter(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.Owner)));
            yield return new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)]));

            yield return new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(RemovedObserver2), nameof(Invoke)));
        }
    }

    private static bool Invoke(bool value, Player target, Player scp)
    {
        if (!value)
            return false;

        new Scp173RemovedObserverEvent(target, scp).InvokeEvent();

        return true;
    }
}