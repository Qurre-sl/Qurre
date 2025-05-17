using JetBrains.Annotations;
using PlayerRoles;
using Qurre.API.Classification.Roles;
using RemoteAdmin;

namespace Qurre.API.Classification.Player;

[PublicAPI]
public sealed class RoleInformation
{
    private readonly Controllers.Player _player;

    internal RoleInformation(Controllers.Player pl)
    {
        _player = pl;
        CachedRole = RoleTypeId.None;
    }

    public PlayerRoleBase Base => _player.ReferenceHub.roleManager.CurrentRole;
    public ServerRoles ServerRoles => _player.ReferenceHub.serverRoles;
    public QueryProcessor QueryProcessor => _player.ReferenceHub.queryProcessor;

    public bool IsAlive => Team != Team.Dead;
    public bool IsScp => Team == Team.SCPs;
    public bool IsHuman => IsAlive && !IsScp;
    public bool IsDirty => _player.ReferenceHub.IsDirty();

    public Scp079? Scp079 { get; internal set; }
    public Scp096? Scp096 { get; internal set; }
    public Scp106? Scp106 { get; internal set; }
    public Scp173? Scp173 { get; internal set; }

    public RoleTypeId CachedRole { get; internal set; }

    public Team Team => _player.Disconnected ? CachedRole.GetTeam() : _player.ReferenceHub.GetTeam();

    public Faction Faction => _player.Disconnected ? CachedRole.GetFaction() : _player.ReferenceHub.GetFaction();

    public RoleTypeId Role
    {
        get => _player.Disconnected ? CachedRole : _player.ReferenceHub.GetRoleId();
        set => _player.ReferenceHub.roleManager.ServerSetRole(value, RoleChangeReason.RemoteAdmin);
    }

    public void SetNew(RoleTypeId newRole, RoleChangeReason reason)
    {
        _player.ReferenceHub.roleManager.ServerSetRole(newRole, reason);
    }

    public void SetNew(RoleTypeId newRole, RoleChangeReason reason, RoleSpawnFlags spawnFlags)
    {
        _player.ReferenceHub.roleManager.ServerSetRole(newRole, reason, spawnFlags);
    }

    public void SetSyncModel(RoleTypeId roleTypeId)
    {
        foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
            _player.ReferenceHub.connectionToClient.Send(new RoleSyncInfo(_player.ReferenceHub, roleTypeId,
                referenceHub));
    }
}