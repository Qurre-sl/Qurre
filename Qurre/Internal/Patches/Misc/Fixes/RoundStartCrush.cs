using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using PlayerRoles;
using PlayerRoles.RoleAssign;
using Log = Qurre.API.Log;

namespace Qurre.Internal.Patches.Misc.Fixes;

[HarmonyPatch(typeof(HumanSpawner), nameof(HumanSpawner.AssignHumanRoleToRandomPlayer))]
internal static class RoundStartCrush
{
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // RoleTypeId role
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RoundStartCrush), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static void Invoke(RoleTypeId role)
    {
        try
        {
            HumanSpawner.Candidates.Clear();
            int num1 = int.MaxValue;
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
                try
                {
                    if (!RoleAssigner.CheckPlayer(allHub)) continue;
                    HumanSpawner.RoleHistory orAdd = HumanSpawner.History.GetOrAdd<string, HumanSpawner.RoleHistory>(
                        allHub.authManager.UserId,
                        (Func<HumanSpawner.RoleHistory>)(() => new HumanSpawner.RoleHistory()));
                    int num2 = 0;
                    for (int index = 0; index < 5; ++index)
                        if (orAdd.History[index] == role)
                            ++num2;

                    if (num2 <= num1)
                    {
                        if (num2 < num1)
                            HumanSpawner.Candidates.Clear();
                        HumanSpawner.Candidates.Add(allHub);
                        num1 = num2;
                    }
                }
                catch (Exception err)
                {
                    Log.Warn(err);
                }

            if (HumanSpawner.Candidates.Count == 0)
                return;
            ReferenceHub referenceHub = HumanSpawner.Candidates.RandomItem<ReferenceHub>();
            referenceHub.roleManager.ServerSetRole(role, RoleChangeReason.RoundStart);
            HumanSpawner.History[referenceHub.authManager.UserId].RegisterRole(role);
        }
        catch (Exception err)
        {
            Log.Warn(err);
        }
    }
}