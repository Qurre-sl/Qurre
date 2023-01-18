using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using PlayerRoles;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using UnityEngine;

namespace Qurre.Internal.Patches.Player.Role
{
    [HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.InitializeNewRole))]
    internal static class Spawn
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new (instructions);

            list.InsertRange(
                list.Count - 2, new[]
                {
                    new (OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Spawn), nameof(Invoke)))
                });

            return list.AsEnumerable();
        }

        private static void Invoke(PlayerRoleManager instance, RoleTypeId role)
        {
            Transform transform = instance.CurrentRole.transform;
            API.Player pl = instance.Hub.GetPlayer();

            if (pl is null)
            {
                return;
            }

            SpawnEvent ev = new (pl, role, transform.position, transform.rotation.eulerAngles);
            ev.InvokeEvent();

            pl.MovementState.Position = ev.Position;
            pl.MovementState.Rotation = ev.Rotation;
        }
    }
}