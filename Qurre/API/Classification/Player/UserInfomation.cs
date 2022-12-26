﻿namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    public struct UserInfomation
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

        public int Id => _player.ReferenceHub.PlayerId;
        public string UserId
        {
            get
            {
                if (_player.Bot)
                {
                    if (_ui is null) _ui = $"7{UnityEngine.Random.Range(0, 99999999)}{UnityEngine.Random.Range(0, 99999999)}@bot";
                    return _ui;
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
        public string NickName
        {
            get
            {
                try { return _player.ReferenceHub.nicknameSync.Network_myNickSync; }
                catch { return _nick; }
            }
            set => _player.ReferenceHub.nicknameSync.Network_myNickSync = value;
        }
        public string DisplayName => _player.ReferenceHub.nicknameSync.Network_displayName;
        public string CustomInfo => _player.ReferenceHub.nicknameSync.Network_customPlayerInfoString;
        public string RoleName => _player.ReferenceHub.serverRoles.Network_myText;
        public string RoleColor => _player.ReferenceHub.serverRoles.Network_myColor;
    }
}