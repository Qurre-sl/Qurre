using Hints;
using JetBrains.Annotations;
using Mirror;
using RoundRestarting;

namespace Qurre.API.Classification.Player;

[PublicAPI]
public sealed class Client
{
    private readonly Controllers.Player _player;

    internal Client(Controllers.Player pl)
    {
        _player = pl;
    }

    public HintDisplay HintDisplay => _player.ReferenceHub.hints;

    public void ShowHint(string text, float duration = 1f, HintEffect[]? effect = null)
    {
        HintDisplay.Show(new TextHint(text, [new StringHintParameter(string.Empty)], effect, duration));
    }

    public Controllers.Broadcast Broadcast(string message, ushort time, bool instant = false)
    {
        return Broadcast(time, message, instant);
    }

    public Controllers.Broadcast Broadcast(ushort time, string message, bool instant = false)
    {
        Controllers.Broadcast bc = new(_player, message, time);
        _player.Broadcasts.Add(bc, instant);
        return bc;
    }

    public void SendConsole(string message, string color)
    {
        _player.ReferenceHub.gameConsoleTransmission.SendToClient(message, color);
    }

    public void Disconnect(string? reason = null)
    {
        ServerConsole.Disconnect(_player.GameObject, string.IsNullOrEmpty(reason) ? string.Empty : reason);
    }

    public void Reconnect(float timeout = 0.1f)
    {
        _player.Connection.Send(new RoundRestartMessage(RoundRestartType.FullRestart, timeout, 0, true, false));
    }

    public void Redirect(ushort port, float timeout = 0.1f)
    {
        _player.Connection.Send(new RoundRestartMessage(RoundRestartType.RedirectRestart, timeout, port, true, false));
    }

    public void DimScreen()
    {
        //RoundSummary.singleton.RpcDimScreen();

        const ushort functionHashCode = unchecked((ushort)-1745793588);
        RoundSummary? component = RoundSummary.singleton;
        NetworkWriterPooled? writer = NetworkWriterPool.Get();

        RpcMessage rpcMessage = new()
        {
            netId = component.netId,
            componentIndex = component.ComponentIndex,
            functionHash = functionHashCode,
            payload = writer.ToArraySegment()
        };

        using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
        networkWriterPooled.Write(rpcMessage);
        _player.ReferenceHub.networkIdentity.connectionToClient.Send(rpcMessage);

        NetworkWriterPool.Return(writer);
    }

    public void ShakeScreen(bool achieve = false)
    {
        //AlphaWarheadController.Singleton.RpcShake(achieve);

        const ushort functionHashCode = unchecked((ushort)1208415683);
        AlphaWarheadController? component = AlphaWarheadController.Singleton;
        NetworkWriterPooled? writer = NetworkWriterPool.Get();
        writer.WriteBool(achieve);

        RpcMessage rpcMessage = new()
        {
            netId = component.netId,
            componentIndex = component.ComponentIndex,
            functionHash = functionHashCode,
            payload = writer.ToArraySegment()
        };

        using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
        networkWriterPooled.Write(rpcMessage);
        _player.ReferenceHub.networkIdentity.connectionToClient.Send(rpcMessage);

        NetworkWriterPool.Return(writer);
    }
}