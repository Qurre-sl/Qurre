using Qurre.Internal.Misc;

namespace Qurre.API.Classification.Player
{
    public class Administrative
    {
        private readonly API.Player _player;

        internal Administrative(API.Player pl) => _player = pl;

        public ServerRoles ServerRoles => _player.ReferenceHub.serverRoles;

        public bool RemoteAdmin => ServerRoles.RemoteAdmin;
        public bool GlobalRemoteAdmin => ServerRoles.RemoteAdminMode == ServerRoles.AccessMode.GlobalAccess;

        public UserGroup Group
        {
            get => ServerRoles.Group;
            set => ServerRoles.SetGroup(value, false, false, value.Cover);
        }

        public string GroupName
        {
            get => ServerStatic.GetPermissionsHandler()._members.TryGetValue(_player.UserInfomation.UserId, out string groupName) ? groupName : null;
            set => ServerStatic.GetPermissionsHandler()._members[_player.UserInfomation.UserId] = value;
        }

        public string RoleName
        {
            get => ServerRoles.Network_myText;
            set => ServerRoles.Network_myText = value;
        }

        public string RoleColor
        {
            get => ServerRoles.Network_myColor;
            set => ServerRoles.Network_myColor = value;
        }

        public void RaLogin()
        {
            ServerRoles.RemoteAdmin = true;
            ServerRoles.Permissions = ServerRoles._globalPerms;
            ServerRoles.RemoteAdminMode = GlobalRemoteAdmin ? ServerRoles.AccessMode.GlobalAccess : ServerRoles.AccessMode.PasswordOverride;
            ServerRoles.TargetOpenRemoteAdmin(false);
        }

        public void RaLogout()
        {
            ServerRoles.RemoteAdmin = false;
            ServerRoles.RemoteAdminMode = ServerRoles.AccessMode.LocalAccess;
            ServerRoles.TargetCloseRemoteAdmin();
        }

        public void Ban(long duration, string reason, string issuer = "API")
            => BanPlayer.BanUser(_player.ReferenceHub, new BanSender(issuer), reason, duration);

        public void Kick(string reason, string issuer = "API")
            => BanPlayer.KickUser(_player.ReferenceHub, new BanSender(issuer), reason);
    }
}