using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.AlphaEvents;

[HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdSwitchAWButton))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class UnlockPanel
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        LocalBuilder @event = generator.DeclareLocal(typeof(UnlockPanelEvent));

        List<CodeInstruction> list = [..instructions];
        list.Last().labels.Add(retLabel);

        int index = list.FindIndex(ins =>
            ins.opcode == OpCodes.Call && ins.operand is MethodBase { Name: nameof(PlayerInteract.OnInteract) });

        if (index < 1)
        {
            Log.Error($"Creating Patch error: <Alpha> [UnlockPanel]: Index - {index} < 1");
            return list.AsEnumerable();
        }

        list.InsertRange(index - 1,
        [
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index - 1]), // this
            new CodeInstruction(OpCodes.Ldfld,
                AccessTools.Field(typeof(PlayerInteract), nameof(PlayerInteract._hub))), // ReferenceHub
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),
            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UnlockPanelEvent))[0]),
            new CodeInstruction(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            // if(!@event.Allowed) return;
            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(UnlockPanelEvent), nameof(UnlockPanelEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);

        return list.AsEnumerable();
    }
}
/*
 * ...
 * < if(...) return; >
 *
 * var @event = new UnlockPanelEvent(this._hub.GetPlayer());
 * @event.InvokeEvent();
 * if(!@event.Allowed) return;
 *
 * this.OnInteract();
 * ...
 */