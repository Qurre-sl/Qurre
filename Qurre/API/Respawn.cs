using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using Qurre.API.Objects;
using Respawning;
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

        static public Vector3 GetPosition(RoleTypeId role)
            => GetSpawnPoint(role).Position;
        static public Transform GetTransform(RoleTypeId role)
        {
            GameObject obj = new("SpawnPoint (Clone)");
            obj.transform.position = GetSpawnPoint(role).Position;
            return obj.transform;
        }
        static public SpawnPoint GetSpawnPoint(RoleTypeId role, bool checkFlags = true)
        {
            var roleBase = Server.Host.ReferenceHub.roleManager.GetRoleBase(role);

            if (roleBase is not IFpcRole fpc)
                return new(Vector3.zero, 0);

            if (fpc.SpawnpointHandler is null)
                return new(Vector3.zero, 0);

            if (!fpc.SpawnpointHandler.TryGetSpawnpoint(out Vector3 pos, out float horizontal))
                return new(Vector3.zero, 0);

            if (checkFlags && !roleBase.ServerSpawnFlags.HasFlag(RoleSpawnFlags.UseSpawnpoint))
                return new(Vector3.zero, 0);

            return new(pos, horizontal);
        }
    }
}