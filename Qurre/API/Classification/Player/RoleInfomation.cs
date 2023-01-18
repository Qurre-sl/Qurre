using PlayerRoles;
using Qurre.API.Classification.Roles;
using RemoteAdmin;
using Respawning;

namespace Qurre.API.Classification.Player
{
    public class RoleInfomation
    {
        private readonly API.Player _player;

        internal RoleInfomation(API.Player pl) => _player = pl;

        public ServerRoles ServerRoles => _player.ReferenceHub.serverRoles;
        public QueryProcessor QueryProcessor => _player.ReferenceHub.queryProcessor;

        public bool IsAlive => _player.ReferenceHub.IsAlive();
        public bool IsScp => _player.ReferenceHub.IsSCP();
        public bool IsHuman => _player.ReferenceHub.IsHuman();
        public bool IsDirty => _player.ReferenceHub.IsDirty();

        public Scp079 Scp079 { get; internal set; }

        public RoleTypeId Role
        {
            get => _player.ReferenceHub.GetRoleId();
            set => _player.ReferenceHub.roleManager.ServerSetRole(value, RoleChangeReason.RemoteAdmin);
        }

        public Team Team => _player.ReferenceHub.GetTeam();

        public float TimeForNextSequence => RespawnManager.Singleton._timeForNextSequence;

        public void SetNew(RoleTypeId newRole, RoleChangeReason reason)
            => _player.ReferenceHub.roleManager.ServerSetRole(newRole, reason);

        public void SetSyncModel(RoleTypeId roleTypeId)
        {
            foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
            {
                _player.ReferenceHub.connectionToClient.Send(new RoleSyncInfo(_player.ReferenceHub, roleTypeId, referenceHub));
            }
        }
    }
}