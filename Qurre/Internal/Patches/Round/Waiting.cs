using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Round
{
    using Qurre.Events.Structs;

    [HarmonyPatch]
    static class Waiting
    {
        static MethodBase TargetMethod() =>
            AccessTools.Method(AccessTools.FirstInner(typeof(CharacterClassManager), (Type x) => x.Name.Contains("<Init>")), "MoveNext");
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool need = true;
            foreach (var ins in instructions)
            {
                if (need && ins.opcode == OpCodes.Ldstr && ins.operand as string == "Waiting for players...")
                {
                    need = false;
                    yield return new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(WaitingEvent))[0]);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent)));
                }
                yield return ins;
            }
        }
    }
}