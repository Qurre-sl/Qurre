using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.Round
{
    [HarmonyPatch]
    internal static class Waiting
    {
        private static MethodBase TargetMethod() =>
            AccessTools.Method(AccessTools.FirstInner(typeof(CharacterClassManager), x => x.Name.Contains("<Init>")), "MoveNext");

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var need = true;

            foreach (CodeInstruction ins in instructions)
            {
                if (need && ins.opcode == OpCodes.Ldstr && ins.operand as string == "Waiting for players...")
                {
                    need = false;
                    yield return new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(WaitingEvent))[0]);
                    yield return new (OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent)));
                }

                yield return ins;
            }
        }
    }
}