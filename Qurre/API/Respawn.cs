using PlayerRoles;
using Respawning;
using UnityEngine;

namespace Qurre.API
{
    public static class Respawn
    {
        public static SpawnableTeamType NextKnownTeam => RespawnManager.Singleton.NextKnownTeam;

        public static float NtfTickets
        {
            get => RespawnTokensManager.Counters[1].Amount;
            set => RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, value);
        }

        public static float ChaosTickets
        {
            get => RespawnTokensManager.Counters[0].Amount;
            set => RespawnTokensManager.GrantTokens(SpawnableTeamType.ChaosInsurgency, value);
        }

        public static Transform GetTransform(RoleTypeId role)
            => Server.Host.ReferenceHub.roleManager.GetRoleBase(role).transform;

        public static Vector3 GetPosition(RoleTypeId role)
            => GetTransform(role).position;
    }
}