using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.Internal.Patches.Player.Interact
{
    //[HarmonyDebug]
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract))]
    static class InteractDoor
    {
        [HarmonyTranspiler] // to do later
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new(instructions);
            return list.AsEnumerable();
        }
    }
}