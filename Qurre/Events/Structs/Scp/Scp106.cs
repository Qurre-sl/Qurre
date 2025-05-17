using JetBrains.Annotations;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp106AttackEvent : IBaseEvent
{
    private const uint EventID = ScpEvents.Scp106Attack;

    internal Scp106AttackEvent(Player attacker, Player target)
    {
        Attacker = attacker;
        Target = target;
        Allowed = true;
    }

    public Player Attacker { get; }
    public Player Target { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}