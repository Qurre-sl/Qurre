using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.Player.Network
{
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.UserCode_CmdServerSignatureComplete))]
    internal static class Join
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;

            foreach (CodeInstruction ins in instructions)
            {
                yield return ins;

                if (!found && ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase && methodBase.Name == nameof(ServerRoles.RefreshPermissions))
                {
                    found = true;
                    yield return new (OpCodes.Ldarg_0);
                    yield return new (OpCodes.Ldfld, AccessTools.Field(typeof(ServerRoles), nameof(ServerRoles._hub)));
                    yield return new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(API.Player))[1]);
                    yield return new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(JoinEvent))[0]);
                    yield return new (OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent)));
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