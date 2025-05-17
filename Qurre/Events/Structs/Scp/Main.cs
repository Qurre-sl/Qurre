using JetBrains.Annotations;
using Qurre.API.Entities.Characters;
using Qurre.API.Enums;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class ScpAttackEvent : ICancellableEvent
{
    internal ScpAttackEvent(Player attacker, Player target, ScpAttackTypes type)
    {
        Attacker = attacker;
        Target = target;
        Type = type;
    }

    /// <inheritdoc />
    public uint EventId { get; } = ScpEvents.Attack;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Attacker { get; }
    public Player Target { get; }
    public ScpAttackTypes Type { get; }
    public float Damage { get; set; }
}