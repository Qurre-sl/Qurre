using HarmonyLib;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp079.Rewards;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Scp.Scp079
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(Scp079RewardManager), nameof(Scp079RewardManager.GrantExp))]
    static class GetExp
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0); // instance [Scp079Role]
            yield return new CodeInstruction(OpCodes.Ldarg_1); // reward [int]
            yield return new CodeInstruction(OpCodes.Ldarg_2); // gainReason [Scp079HudTranslation]
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GetExp), nameof(GetExp.Invoke)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        static void Invoke(Scp079Role instance, int reward, Scp079HudTranslation gainReason)
        {
            try
            {
                Scp079RewardManager.RefreshCache();

                if (!instance.TryGetOwner(out var hub))
                    return;

                Scp079GetExpEvent ev = new(hub.GetPlayer(), gainReason, reward);
                ev.InvokeEvent();

                if (!ev.Allowed)
                    return;

                ev.Player.RoleInfomation.Scp079.TierManager.ServerGrantExperience(ev.Amount, ev.Type);
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <SCPs> {{Scp079}} [GetExp]: {e}\n{e.StackTrace}");
            }
        }
    }
}