using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Utils;

[HarmonyPatch(typeof(FpcFromClientMessage), nameof(FpcFromClientMessage.ProcessMessage))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Jump
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> list = [.. instructions];

        list.InsertRange(list.Count - 1, [
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Jump), nameof(Invoke)))
        ]);

        return list.AsEnumerable();
    }

    private static void Invoke(ReferenceHub hub)
    {
        Player? pl = hub.GetPlayer();

        if (pl is null)
            return;

        new JumpEvent(pl).InvokeEvent();
    }
}