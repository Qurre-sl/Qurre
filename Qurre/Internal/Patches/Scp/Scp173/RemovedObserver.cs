/* это мое чудо крашит сервер, надо будет потом посмотреть что я в il коде выдал и переписать по нормальному

namespace Qurre.Internal.Patches.Scp.Scp173;

using HarmonyLib;
using PlayerRoles.PlayableScps.Scp173;
using Qurre.API;
using Qurre.Events.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using Qurre.Internal.EventsManager;

[HarmonyPatch(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.CheckRemovedPlayer))]
static class RemovedObserver1
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> list = new(instructions);

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
            methodBase.Name.Contains("set_CurrentObservers")) + 1;

        if (index < 1)
        {
            Log.Error($"Creating Patch error: <SCPs> {{Scp173}} [RemovedObserver #1]: Index - {index} < 1");
            return list.AsEnumerable();
        }

        list.InsertRange(index, new CodeInstruction[]
        {
            new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(list[index]), // target [ReferenceHub]
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new Type[] { typeof(ReferenceHub) })),

            new CodeInstruction(OpCodes.Ldarg_0), // instance [Scp173ObserversTracker]
            new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.Owner))),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new Type[] { typeof(ReferenceHub) })),

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(Scp173RemovedObserverEvent))[0]), // new Scp173RemovedObserverEvent(...);
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))), // .InvokeEvent();
        });

        return list.AsEnumerable();
    }
}

[HarmonyPatch(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.UpdateObserver))]
static class RemovedObserver2
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        foreach (CodeInstruction ins in instructions)
        {
            yield return ins;

            if (ins.opcode == OpCodes.Callvirt && ins.operand is not null && ins.operand is MethodBase methodBase &&
            methodBase.Name.Contains("Remove"))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new Type[] { typeof(ReferenceHub) }));

                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.Owner)));
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new Type[] { typeof(ReferenceHub) }));

                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RemovedObserver2), nameof(RemovedObserver2.Invoke)));
            }
        }

        yield break;
    }

    static bool Invoke(bool value, ReferenceHub target, ReferenceHub scp)
    {
        if (!value)
            return false;

        new Scp173RemovedObserverEvent(target.GetPlayer(), scp.GetPlayer()).InvokeEvent();

        return true;
    }
}*/