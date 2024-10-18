using JetBrains.Annotations;
using Qurre.API;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class CuffEvent : IBaseEvent
{
    internal CuffEvent(Player target, Player cuffer)
    {
        Target = target;
        Cuffer = cuffer;
        Allowed = true;
    }

    public Player Target { get; }
    public Player Cuffer { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = PlayerEvents.Cuff;
}

[PublicAPI]
public class UnCuffEvent : IBaseEvent
{
    internal UnCuffEvent(Player target, Player cuffer)
    {
        Target = target;
        Cuffer = cuffer;
        Allowed = true;
    }

    public Player Target { get; }
    public Player Cuffer { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = PlayerEvents.UnCuff;
}

[PublicAPI]
public class ChangeSpectateEvent : IBaseEvent
{
    internal ChangeSpectateEvent(Player player, Player? old, Player? @new)
    {
        Player = player;
        Old = old;
        New = @new;
    }

    public Player Player { get; }
    public Player? Old { get; }
    public Player? New { get; }
    public uint EventId { get; } = EventID;

    private const uint EventID = PlayerEvents.ChangeSpectate;
}