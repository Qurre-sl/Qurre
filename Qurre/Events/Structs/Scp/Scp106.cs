using JetBrains.Annotations;
using Qurre.API;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp106AttackEvent : IBaseEvent
{
    internal Scp106AttackEvent(Player attacker, Player target)
    {
        Attacker = attacker;
        Target = target;
        Allowed = true;
    }

    public Player Attacker { get; }
    public Player Target { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = ScpEvents.Scp106Attack;
}