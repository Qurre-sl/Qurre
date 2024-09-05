namespace Qurre.API.Classification.Player;

using PlayerRoles;
using Qurre.API;
using Qurre.API.Classification.Roles;
using RemoteAdmin;
using Respawning;

public sealed class RoleInformation
{
    readonly Player _player;
    internal RoleTypeId cachedRole;
    internal RoleInformation(Player pl)
    {
        _player = pl;
        cachedRole = RoleTypeId.None;
    }

    public PlayerRoleBase Base => _player.ReferenceHub.roleManager.CurrentRole;
    public ServerRoles ServerRoles => _player.ReferenceHub.serverRoles;
    public QueryProcessor QueryProcessor => _player.ReferenceHub.queryProcessor;

    public bool IsAlive => Team != Team.Dead;
    public bool IsScp => Team == Team.SCPs;
    public bool IsHuman => IsAlive && !IsScp;
    public bool IsDirty => _player.ReferenceHub.IsDirty();

    public Scp079 Scp079 { get; internal set; }
    public Scp096 Scp096 { get; internal set; }
    public Scp106 Scp106 { get; internal set; }
    public Scp173 Scp173 { get; internal set; }

    public RoleTypeId CachedRole
        => cachedRole;

    public RoleTypeId Role
    {
        get
        {
            if (_player.Disconnected)
                return cachedRole;
            return _player.ReferenceHub.GetRoleId();
        }
        set => _player.ReferenceHub.roleManager.ServerSetRole(value, RoleChangeReason.RemoteAdmin);
    }
    public Team Team
    {
        get
        {
            if (_player.Disconnected)
                return cachedRole.GetTeam();
            return _player.ReferenceHub.GetTeam();
        }
    }
    public Faction Faction
    {
        get
        {
            if (_player.Disconnected)
                return cachedRole.GetFaction();
            return _player.ReferenceHub.GetFaction();
        }
    }
    public float TimeForNextSequence
    {
        get => RespawnManager.Singleton._timeForNextSequence;
    }

    public void SetNew(RoleTypeId newRole, RoleChangeReason reason)
        => _player.ReferenceHub.roleManager.ServerSetRole(newRole, reason);
    public void SetNew(RoleTypeId newRole, RoleChangeReason reason, RoleSpawnFlags spawnFlags)
        => _player.ReferenceHub.roleManager.ServerSetRole(newRole, reason, spawnFlags);

    public void SetSyncModel(RoleTypeId roleTypeId)
    {
        foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
            _player.ReferenceHub.connectionToClient.Send(new RoleSyncInfo(_player.ReferenceHub, roleTypeId, referenceHub), 0);
    }
}