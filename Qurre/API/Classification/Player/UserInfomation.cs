namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    public struct UserInfomation
    {
        private readonly Player _player;
        internal UserInfomation(Player pl) => _player = pl;

        public string UserId
        {
            get => _player.ReferenceHub.characterClassManager.UserId;
            set => _player.ReferenceHub.characterClassManager.UserId = value;
        }
        public string NickName
        {
            get => _player.ReferenceHub.nicknameSync.Network_myNickSync;
            set => _player.ReferenceHub.nicknameSync.Network_myNickSync = value;
        }
        public string DisplayName => _player.ReferenceHub.nicknameSync.Network_displayName;
        public string CustomInfo => _player.ReferenceHub.nicknameSync.Network_customPlayerInfoString;
        public string RoleName => _player.ReferenceHub.serverRoles.Network_myText;
        public string RoleColor => _player.ReferenceHub.serverRoles.Network_myColor;
    }
}