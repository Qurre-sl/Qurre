using PlayerRoles;
using Respawning;
namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    public class RoleInfomation
    {
        private readonly Player _player;
        internal RoleInfomation(Player pl) => _player = pl;

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
    }
}