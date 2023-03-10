using PlayerRoles;
using Respawning;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    using Qurre.API.Classification.Roles;
    using RemoteAdmin;

    public sealed class RoleInfomation
    {
        private readonly Player _player;
        internal RoleInfomation(Player pl) => _player = pl;

        public ServerRoles ServerRoles => _player.ReferenceHub.serverRoles;
        public QueryProcessor QueryProcessor => _player.ReferenceHub.queryProcessor;

        public bool IsAlive => _player.ReferenceHub.IsAlive();
        public bool IsScp => _player.ReferenceHub.IsSCP();
        public bool IsHuman => _player.ReferenceHub.IsHuman();
        public bool IsDirty => _player.ReferenceHub.IsDirty();

        public Scp079 Scp079 { get; internal set; }
        public Scp106 Scp106 { get; internal set; }
        public Scp173 Scp173 { get; internal set; }

        public RoleTypeId Role
        {
            get => _player.ReferenceHub.GetRoleId();
            set => _player.ReferenceHub.roleManager.ServerSetRole(value, RoleChangeReason.RemoteAdmin);
        }
        public Team Team
        {
            get => _player.ReferenceHub.GetTeam();
        }
        public Faction Faction
        {
            get => _player.ReferenceHub.GetFaction();
        }
        public float TimeForNextSequence
        {
            get => RespawnManager.Singleton._timeForNextSequence;
        }

        public void SetNew(RoleTypeId newRole, RoleChangeReason reason)
            => _player.ReferenceHub.roleManager.ServerSetRole(newRole, reason);

        public void SetSyncModel(RoleTypeId roleTypeId)
        {
            foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
                _player.ReferenceHub.connectionToClient.Send(new RoleSyncInfo(_player.ReferenceHub, roleTypeId, referenceHub), 0);
        }
    }
}