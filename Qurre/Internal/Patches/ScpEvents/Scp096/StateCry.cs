using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp096;
using Qurre.API;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.ScpEvents.Scp096;

[HarmonyPatch(typeof(Scp096TryNotToCryAbility), nameof(Scp096TryNotToCryAbility.IsActive), MethodType.Setter)]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class StateCry
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        LocalBuilder @event = generator.DeclareLocal(typeof(Scp096SetStateEvent));

        List<CodeInstruction> list = [..instructions];
        list.Last().labels.Add(retLabel);

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Newobj &&
                                          ins.operand is MethodBase { DeclaringType.FullName: not null } methodBase &&
                                          methodBase.DeclaringType.FullName.Contains("TryNotCry")) - 1;

        if (index < 0)
        {
            Log.Error($"Creating Patch error: <SCPs> {{Scp096}} [StateCry // TryNotCry]: Index - {index} < 0");
            return list.AsEnumerable();
        }

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(list[index]), // hub [ReferenceHub]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Ldc_I4_3), // 3 [Scp096State]

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(Scp096SetStateEvent))[0]),
            new CodeInstruction(OpCodes.Stloc_S, @event.LocalIndex), // var @event = ...;

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // if (!@event.Allowed) return;
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(Scp096SetStateEvent), nameof(Scp096SetStateEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);

        index = list.FindIndex(ins => ins.opcode == OpCodes.Newobj &&
                                      ins.operand is MethodBase { DeclaringType.FullName: not null } methodBase &&
                                      methodBase.DeclaringType.FullName.Contains("StartCrying")) - 1;

        if (index < 0)
        {
            Log.Error($"Creating Patch error: <SCPs> {{Scp096}} [StateCry // StartCrying]: Index - {index} < 0");
            return list.AsEnumerable();
        }

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Ldloc_1).MoveLabelsFrom(list[index]), // hub2 [ReferenceHub]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Ldc_I4_4), // 4 [Scp096State]

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(Scp096SetStateEvent))[0]),
            new CodeInstruction(OpCodes.Stloc_S, @event.LocalIndex), // var @event = ...;

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // if (!@event.Allowed) return;
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(Scp096SetStateEvent), nameof(Scp096SetStateEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);

        return list.AsEnumerable();
    }
}