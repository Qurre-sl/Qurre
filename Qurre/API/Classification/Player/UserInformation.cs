﻿namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    public sealed class UserInformation
    {
        private string _ui;
        private readonly string _nick = "";
        private readonly Player _player;
        internal UserInformation(Player pl)
        {
            _player = pl;
            _ui = pl.AuthManager.UserId;
            try { _nick = pl.ReferenceHub.nicknameSync.Network_myNickSync; } catch { }
        }

        public NicknameSync NicknameSync => _player.ReferenceHub.nicknameSync;

        public string Ip => _player.ClassManager.connectionToClient.address;
        public int Id => _player.ReferenceHub.PlayerId;
        public uint NetId => _player.ReferenceHub.netId;
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
                    string _ = _player.AuthManager.UserId;
                    if (_.Contains("@"))
                    {
                        _ui = _;
                        return _;
                    }
                    return _ui;
                }
                catch { return _ui; }
            }
            set => _player.AuthManager.UserId = value;
        }
        public string Nickname
        {
            get
            {
                try { return NicknameSync.Network_myNickSync; }
                catch { return _nick; }
            }
            set
            {
                NicknameSync.Network_myNickSync = value;

                foreach (Player item in Player.List)
                {
                    Network.SendSpawnMessage?.Invoke(null, new object[2]
                    {
                            _player.ClassManager.netIdentity,
                            item.Connection
                    });
                }
            }
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

        public bool DoNotTrack => _player.AuthManager.DoNotTrack;
    }
}