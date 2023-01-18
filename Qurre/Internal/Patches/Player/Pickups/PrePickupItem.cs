using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Searching;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.Player.Pickups
{
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.ValidateStart))]
    internal static class PrePickupItem
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            LocalBuilder @event = generator.DeclareLocal(typeof(PrePickupItemEvent));

            List<CodeInstruction> list = new (instructions);

            Label _label = generator.DefineLabel();
            List<Label> labels = list[list.Count - 2].ExtractLabels();
            list[list.Count - 2].labels.Add(_label);

            list.InsertRange(
                list.Count - 2, new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
                    new (OpCodes.Ldfld, AccessTools.Field(typeof(SearchCompletor), nameof(SearchCompletor.Hub))),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new[] { typeof(ReferenceHub) })),

                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SearchCompletor), nameof(SearchCompletor.TargetPickup))),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pickup), nameof(Pickup.SafeGet))),

                    new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(PrePickupItemEvent))[0]),
                    new CodeInstruction(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

                    new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                    // if(!@event.Allowed) return;
                    new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PrePickupItemEvent), nameof(PrePickupItemEvent.Allowed))),
                    new CodeInstruction(OpCodes.Brtrue, _label),
                    new CodeInstruction(OpCodes.Ldc_I4_0), // false [if boolean]
                    new CodeInstruction(OpCodes.Br, retLabel)
                });

            return list.AsEnumerable();
        }
        /*
         * ...
         * PrePickupItemEvent @event = new(this.Hub.GetPlayer(), Pickup.SafeGet(this.TargetPickup));
         * @event.InvokeEvent();
         * 
         * if(!@event.Allowed)
         *     return false;
         * 
         * return true;
        */
    }
}