using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp173;
using Qurre.API;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.ScpEvents.Scp173;

[HarmonyPatch(typeof(Scp173BreakneckSpeedsAbility), nameof(Scp173BreakneckSpeedsAbility.IsActive), MethodType.Setter)]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class EnableSpeed
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        LocalBuilder @event = generator.DeclareLocal(typeof(Scp173EnableSpeedEvent));

        List<CodeInstruction> list = [..instructions];
        list.Last().labels.Add(retLabel);

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Call &&
                                          ins.operand is MethodBase methodBase &&
                                          methodBase.Name.Contains("ExecuteEvent")) + 3;

        if (index < 3)
        {
            Log.Error($"Creating Patch error: <SCPs> {{Scp173}} [EnableSpeed]: Index - {index} < 3");
            return list.AsEnumerable();
        }

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index]), // instance [Scp173BreakneckSpeedsAbility]
            new CodeInstruction(OpCodes.Call,
                AccessTools.PropertyGetter(typeof(Scp173BreakneckSpeedsAbility),
                    nameof(Scp173BreakneckSpeedsAbility.Owner))),
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Ldarg_1), // value [bool]

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(Scp173EnableSpeedEvent))[0]),
            new CodeInstruction(OpCodes.Stloc_S, @event.LocalIndex), // var @event = ...;

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // if (!@event.Allowed) return;
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(Scp173EnableSpeedEvent), nameof(Scp173EnableSpeedEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);

        return list.AsEnumerable();
    }
}
/*
 * ...
 *
 * Scp173EnableSpeedEvent @event = new(instance.Owner.GetPlayer(), value));
 * @event.InvokeEvent();
 *
 * if (!@event.Allowed) {
 *     return;
 * }
 *
 * ...
 */