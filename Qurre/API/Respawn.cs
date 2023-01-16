using PlayerRoles;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Qurre.API
{
    static public class Respawn
    {
        static public SpawnableTeamType NextKnownTeam => RespawnManager.Singleton.NextKnownTeam;

        static public float NtfTickets
        {
            get => RespawnTokensManager.Counters[1].Amount;
            set => RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, value);
        }
        static public float ChaosTickets
        {
            get => RespawnTokensManager.Counters[0].Amount;
            set => RespawnTokensManager.GrantTokens(SpawnableTeamType.ChaosInsurgency, value);
        }

        static public Transform GetTransform(RoleTypeId role)
            => Server.Host.ReferenceHub.roleManager.GetRoleBase(role).transform;
        static public Vector3 GetPosition(RoleTypeId role)
            => GetTransform(role).position;
    }
}