using JetBrains.Annotations;
using Qurre.API;
using Qurre.API.Objects;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class ScpAttackEvent : IBaseEvent
{
    internal ScpAttackEvent(Player attacker, Player target, ScpAttackType type)
    {
        Attacker = attacker;
        Target = target;
        Type = type;
        Allowed = true;
    }

    public Player Attacker { get; }
    public Player Target { get; }
    public ScpAttackType Type { get; }
    public float Damage { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = ScpEvents.Attack;
}