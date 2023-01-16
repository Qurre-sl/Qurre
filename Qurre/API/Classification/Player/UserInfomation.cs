namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    public class UserInfomation
    {
        private string _ui;
        private readonly string _nick = "";
        private readonly Player _player;
        internal UserInfomation(Player pl)
        {
            _player = pl;
            _ui = pl.ClassManager.UserId;
            try { _nick = pl.ReferenceHub.nicknameSync.Network_myNickSync; } catch { }
        }

        public NicknameSync NicknameSync => _player.ReferenceHub.nicknameSync;

        public string Ip => _player.ClassManager.connectionToClient.address;
        public int Id => _player.ReferenceHub.PlayerId;
        public string UserId
        {
            get
            {
                if (_player.Bot)
                {
                    return _ui ??= $"7{UnityEngine.Random.Range(0, 99999999)}{UnityEngine.Random.Range(0, 99999999)}@bot";
                }

                try
                {
                    string _ = _player.ClassManager.UserId;
                    if (_.Contains("@"))
                    {
                        _ui = _;
                        return _;
                    }
                    return _ui;
                }
                catch { return _ui; }
            }
            set => _player.ClassManager.UserId = value;
        }
        public string Nickname
        {
            get
            {
                try { return NicknameSync.Network_myNickSync; }
                catch { return _nick; }
            }
            set => NicknameSync.Network_myNickSync = value;
        }
        public string DisplayName
        {
            get => NicknameSync.Network_displayName;
            set => NicknameSync.Network_displayName = value;
        }
        public string CustomInfo
        {
            get => NicknameSync.Network_customPlayerInfoString;
            set => NicknameSync.Network_customPlayerInfoString = value;
        }
        public PlayerInfoArea InfoToShow
        {
            get => NicknameSync.Network_playerInfoToShow;
            set => NicknameSync.Network_playerInfoToShow = value;
        }

        public bool DoNotTrack => _player.ReferenceHub.serverRoles.DoNotTrack;
    }
}