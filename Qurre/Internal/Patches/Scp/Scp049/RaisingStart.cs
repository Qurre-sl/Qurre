namespace Qurre.Internal.Patches.Scp.Scp049;

using HarmonyLib;
using PlayerRoles.PlayableScps.Scp049;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using System.Collections.Generic;
using System.Reflection.Emit;
using static PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility;

[HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerValidateAny))]
static class RaisingStart
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // instance [Scp049ResurrectAbility]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaisingStart), nameof(RaisingStart.Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    static bool Invoke(Scp049ResurrectAbility instance)
    {
        if (instance.CurRagdoll is null)
            return false;

        ReferenceHub target = instance.CurRagdoll.Info.OwnerHub;

        if (target is null)
            return false;

        Scp049RaisingStartEvent @event = new(instance.Owner.GetPlayer(), target.GetPlayer(), instance.CurRagdoll);

        @event.Allowed = instance.IsCloseEnough(instance.CastRole.FpcModule.Position, instance._ragdollTransform.position)
            && IsSpawnableSpectator(target) &&
            instance.CheckMaxResurrections(target) == ResurrectError.None &&
            !instance.AnyConflicts(@event.Ragdoll);

        @event.InvokeEvent();

        return @event.Allowed;
    }
}