using JetBrains.Annotations;
using PlayerRoles.Ragdolls;
using Qurre.API;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp049RaisingStartEvent : IBaseEvent
{
    internal Scp049RaisingStartEvent(Player player, Player target, BasicRagdoll doll)
    {
        Player = player;
        Target = target;
        Ragdoll = doll;
        Allowed = true;
    }

    public Player Player { get; }
    public Player Target { get; }
    public BasicRagdoll Ragdoll { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = ScpEvents.Scp049RaisingStart;
}

[PublicAPI]
public class Scp049RaisingEndEvent : IBaseEvent
{
    internal Scp049RaisingEndEvent(Player player, Player target, BasicRagdoll doll)
    {
        Player = player;
        Target = target;
        Ragdoll = doll;
        Allowed = true;
    }

    public Player Player { get; }
    public Player Target { get; }
    public BasicRagdoll Ragdoll { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = ScpEvents.Scp049RaisingEnd;
}