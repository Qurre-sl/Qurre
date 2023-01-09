using GameCore;
using HarmonyLib;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Spawnpoints;
using PlayerStatsSystem;
using Qurre.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Qurre.Internal.Patches.Player.Class
{
    [HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.InitializeNewRole))]
    static class FixSpawnPoint
    {
        

    }
}
