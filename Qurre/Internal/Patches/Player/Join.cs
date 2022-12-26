using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace Qurre.Internal.Patches.Player
{
    using Qurre.API;
    using Qurre.Events.Structs;

    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.UserCode_CmdServerSignatureComplete))]
    static class Join
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            foreach (var ins in instructions)
            {
                yield return ins;
                if (!found && ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                    methodBase.Name == nameof(ServerRoles.RefreshPermissions))
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ServerRoles), nameof(ServerRoles._hub)));
                    yield return new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(Player))[1]);
                    yield return new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(JoinEvent))[0]);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent)));
                }
            }
        }
        /*
         * ...
         * RefreshPermissions();
         * Qurre.Internal.EventsManager.Loader.InvokeEvent(new Qurre.Events.Structs.JoinEvent(new Qurre.API.Player(this._hub)));
         * _authChallenge = null;
         * ...
        */
    }
}