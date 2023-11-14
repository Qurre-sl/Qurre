using HarmonyLib;
using InventorySystem;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Player.Pickups
{
    using Qurre.API;
    using Qurre.API.Controllers;
    using Qurre.Events.Structs;

    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropItem__UInt16__Boolean))]
    static class DropItem
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            LocalBuilder @event = generator.DeclareLocal(typeof(DropItemEvent));

            List<CodeInstruction> list = new(instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name == nameof(InventoryExtensions.ServerDropItem)) - 2;

            if (index < 0)
            {
                Log.Error($"Creating Patch error: <Player> {{Pickups}} [DropItem]: Index - {index} < 0");
                return list.AsEnumerable();
            }

            list.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index]),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Inventory), nameof(Inventory._hub))),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new[] { typeof(ReferenceHub) })),

                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Item), nameof(Item.SafeGet))),

                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(DropItemEvent))[0]),
                new CodeInstruction(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

                new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                // if(!@event.Allowed) return;
                new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(DropItemEvent), nameof(DropItemEvent.Allowed))),
                new CodeInstruction(OpCodes.Brfalse, retLabel),
            });

            return list.AsEnumerable();
        }
    }
}