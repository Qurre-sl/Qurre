using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CentralAuth;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.PlayerEvents.Network;

[HarmonyPatch(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.ProcessAuthenticationResponse))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Join
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> list = [..instructions];

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Call &&
                                          ins.operand is MethodBase methodBase &&
                                          methodBase.Name.Contains("ExecuteEvent")) - 3;

        if (index < 0)
        {
            Log.Error($"Creating Patch error: <Player> {{Network}} [Join]: Index - {index} < 0");
            return list.AsEnumerable();
        }

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index]), // this
            new CodeInstruction(OpCodes.Ldfld,
                AccessTools.Field(typeof(PlayerAuthenticationManager),
                    nameof(PlayerAuthenticationManager._hub))), // this._hub
            new CodeInstruction(OpCodes.Newobj,
                AccessTools.GetDeclaredConstructors(typeof(Player))[0]), // new Player(this._hub)

            new CodeInstruction(OpCodes.Newobj,
                AccessTools.GetDeclaredConstructors(typeof(JoinEvent))[0]), // new JoinEvent(...)

            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader),
                    nameof(EventsManager.Loader.InvokeEvent))) // InvokeEvent(...);
        ]);

        return list.AsEnumerable();
    }
}
/*
 * ...
 * Qurre.Internal.EventsManager.Loader.InvokeEvent(new Qurre.Events.Structs.JoinEvent(new Qurre.API.Player(this._hub)));
 * ...
 */