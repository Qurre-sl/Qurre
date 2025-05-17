using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp079.Rewards;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.ScpEvents.Scp079;

[HarmonyPatch(typeof(Scp079RewardManager), nameof(Scp079RewardManager.GrantExp))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class GetExp
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // instance [Scp079Role]
        yield return new CodeInstruction(OpCodes.Ldarg_1); // reward [int]
        yield return new CodeInstruction(OpCodes.Ldarg_2); // gainReason [Scp079HudTranslation]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GetExp), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static void Invoke(Scp079Role instance, int reward, Scp079HudTranslation gainReason)
    {
        try
        {
            if (!instance.TryGetOwner(out ReferenceHub? hub))
                return;

            Player? pl = hub.GetPlayer();

            if (pl is null)
                return;

            Scp079GetExpEvent ev = new(pl, gainReason, reward);
            ev.InvokeEvent();

            if (!ev.Allowed)
                return;

            ev.Player.RoleInformation.Scp079?.TierManager?.ServerGrantExperience(ev.Amount, ev.Type);
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <SCPs> {{Scp079}} [GetExp]: {e}\n{e.StackTrace}");
        }
    }
}