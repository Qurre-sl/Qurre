using JetBrains.Annotations;

namespace Qurre.API.Classification.Player;

[PublicAPI]
public sealed class UserInformation
{
    private readonly string _nick;
    private readonly Controllers.Player _player;
    private string _ui;

    internal UserInformation(Controllers.Player pl)
    {
        _player = pl;
        _ui = pl.AuthManager.UserId;
        _nick = pl.ReferenceHub.nicknameSync.Network_myNickSync;
    }

    public NicknameSync NicknameSync => _player.ReferenceHub.nicknameSync;

    public string Ip => _player.ClassManager.connectionToClient.address;
    public int Id => _player.ReferenceHub.PlayerId;
    public uint NetId => _player.ReferenceHub.netId;
    public bool DoNotTrack => _player.AuthManager.DoNotTrack;

    public string DisplayName
    {
        get => NicknameSync.Network_displayName;
        set => NicknameSync.Network_displayName = value;
    }

    public string CustomInfo
    {
        get => NicknameSync.Network_customPlayerInfoString ?? string.Empty;
        set => NicknameSync.Network_customPlayerInfoString = value;
    }

    public PlayerInfoArea InfoToShow
    {
        get => NicknameSync.Network_playerInfoToShow;
        set => NicknameSync.Network_playerInfoToShow = value;
    }

    public string UserId
    {
        get
        {
            try
            {
                string _ = _player.AuthManager.UserId;

                if (!_.Contains("@"))
                    return _ui;

                _ui = _;
                return _;
            }
            catch
            {
                return _ui;
            }
        }
        set => _player.AuthManager.UserId = value;
    }

    public string Nickname
    {
        get
        {
            try
            {
                return NicknameSync.Network_myNickSync;
            }
            catch
            {
                return _nick;
            }
        }
        set
        {
            NicknameSync.Network_myNickSync = value;

            foreach (Controllers.Player item in Controllers.Player.List)
                Network.SendSpawnMessage?.Invoke(null,
                [
                    _player.ClassManager.netIdentity,
                    item.Connection
                ]);
        }
    }
}