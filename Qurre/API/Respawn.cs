using JetBrains.Annotations;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using Qurre.API.Objects;
using Respawning;
using UnityEngine;

namespace Qurre.API;

[PublicAPI]
public static class Respawn
{
    public static SpawnableTeamType NextKnownTeam
        => RespawnManager.Singleton.NextKnownTeam;

    public static float MtfTickets
    {
        get => RespawnTokensManager.Counters[1].Amount;
        set => RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, value);
    }

    public static float ChaosTickets
    {
        get => RespawnTokensManager.Counters[0].Amount;
        set => RespawnTokensManager.GrantTokens(SpawnableTeamType.ChaosInsurgency, value);
    }

    public static Vector3 GetPosition(RoleTypeId role)
    {
        return GetSpawnPoint(role).Position;
    }

    public static Transform GetTransform(RoleTypeId role)
    {
        GameObject obj = new("SpawnPoint (Clone)")
        {
            transform =
            {
                position = GetSpawnPoint(role).Position
            }
        };
        return obj.transform;
    }

    public static SpawnPoint GetSpawnPoint(RoleTypeId role)
    {
        PlayerRoleBase? roleBase = Server.Host.ReferenceHub.roleManager.GetRoleBase(role);

        if (roleBase is not IFpcRole fpc)
            return new SpawnPoint(Vector3.zero, 0);

        if (fpc.SpawnpointHandler is null)
            return new SpawnPoint(Vector3.zero, 0);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!fpc.SpawnpointHandler.TryGetSpawnpoint(out Vector3 pos, out float horizontal))
            return new SpawnPoint(Vector3.zero, 0);

        return new SpawnPoint(pos, horizontal);
    }
}