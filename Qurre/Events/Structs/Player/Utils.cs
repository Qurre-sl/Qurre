using JetBrains.Annotations;
using Qurre.API;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class PressAltEvent : IBaseEvent
{
    internal PressAltEvent(Player player, bool allowed)
    {
        Player = player;
        Allowed = allowed;
    }

    public Player Player { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = PlayerEvents.PressAlt;
}