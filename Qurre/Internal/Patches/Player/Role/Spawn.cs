using HarmonyLib;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace Qurre.Internal.Patches.Player.Role
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.InitializeNewRole))]
    static class Spawn
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new(instructions);

            list.InsertRange(list.Count - 2, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Spawn), nameof(Spawn.Invoke)))
            });

            return list.AsEnumerable();
        }

        static void Invoke(PlayerRoleManager instance, RoleTypeId role)
        {
            Transform transform = ((Component)(object)instance.CurrentRole).transform;
            var pl = instance.Hub.GetPlayer();

            if (pl is null)
                return;

            SpawnEvent ev = new(pl, role, transform.position, transform.rotation.eulerAngles);
            ev.InvokeEvent();

            pl.MovementState.Position = ev.Position;
            pl.MovementState.Rotation = ev.Rotation;
        }
    }
}