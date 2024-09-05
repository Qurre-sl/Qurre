namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    using Qurre.Internal.Misc;

    public sealed class Administrative
    {
        private readonly Player _player;
        internal Administrative(Player pl) => _player = pl;

        public ServerRoles ServerRoles => _player.ReferenceHub.serverRoles;

        public bool RemoteAdmin => ServerRoles.RemoteAdmin;

        public UserGroup Group
        {
            get => ServerRoles.Group;
            set => ServerRoles.SetGroup(value, false, false);
        }

        public string GroupName
        {
            get => ServerStatic.GetPermissionsHandler()._members.TryGetValue(_player.UserInformation.UserId, out string groupName) ? groupName : null;
            set => ServerStatic.GetPermissionsHandler()._members[_player.UserInformation.UserId] = value;
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
            ServerRoles.Permissions = ServerRoles.GlobalPerms;
            //_player.AuthManager.ResetPasswordAttempts();
            ServerRoles.RpcResetFixed();
            ServerRoles.OpenRemoteAdmin();
        }
        public void RaLogout()
        {
            ServerRoles.RemoteAdmin = false;
            //_player.AuthManager.ResetPasswordAttempts();
            ServerRoles.RpcResetFixed();
            ServerRoles.TargetSetRemoteAdmin(false);
        }

        public void Ban(long duration, string reason, string issuer = "API")
            => BanPlayer.BanUser(_player.ReferenceHub, new BanSender(issuer), reason, duration);
        public void Kick(string reason, string issuer = "API")
            => BanPlayer.KickUser(_player.ReferenceHub, new BanSender(issuer), reason);
    }
}