namespace Qurre.Events.Structs;

using Qurre.API;

public class PressAltEvent : IBaseEvent
{
    public uint EventId { get; } = PlayerEvents.PressAlt;

    public Player Player { get; }
    public bool Allowed { get; set; }

    internal PressAltEvent(Player player, bool allowed)
    {
        Player = player;
        Allowed = allowed;
    }
}