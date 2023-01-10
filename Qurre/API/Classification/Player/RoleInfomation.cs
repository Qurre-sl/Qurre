using PlayerRoles;
using Respawning;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    using RemoteAdmin;

    public class RoleInfomation
    {
        private readonly Player _player;
        internal RoleInfomation(Player pl) => _player = pl;

        public ServerRoles ServerRoles => _player.ReferenceHub.serverRoles;
        public QueryProcessor QueryProcessor => _player.ReferenceHub.queryProcessor;

        public bool IsAlive => _player.ReferenceHub.IsAlive();
        public bool IsScp => _player.ReferenceHub.IsSCP();
        public bool IsHuman => _player.ReferenceHub.IsHuman();
        public bool IsDirty => _player.ReferenceHub.IsDirty();

        public RoleTypeId Role
        {
            get => _player.ReferenceHub.GetRoleId();
            set => _player.ReferenceHub.roleManager.ServerSetRole(value, RoleChangeReason.RemoteAdmin);
        }
        public Team Team
        {
            get => _player.ReferenceHub.GetTeam();
        }
        public float TimeForNextSequence
        {
            get => RespawnManager.Singleton._timeForNextSequence;
        }
        public void SetSyncModel(RoleTypeId roleTypeId)
        {
            foreach (global::ReferenceHub referenceHub in global::ReferenceHub.AllHubs)
                _player.ReferenceHub.connectionToClient.Send<RoleSyncInfo>(new RoleSyncInfo(_player.ReferenceHub, roleTypeId, referenceHub), 0);
        }
        public void SetNew(RoleTypeId newRole, RoleChangeReason reason) => _player.ReferenceHub.roleManager.ServerSetRole(newRole, reason);
    }
}