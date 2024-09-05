using PlayerRoles.Ragdolls;
using Qurre.API;

namespace Qurre.Events.Structs;

public class Scp049RaisingStartEvent : IBaseEvent
{
    public uint EventId { get; } = ScpEvents.Scp049RaisingStart;

    public Player Player { get; }
    public Player Target { get; }
    public BasicRagdoll Ragdoll { get; }
    public bool Allowed { get; set; }

    internal Scp049RaisingStartEvent(Player player, Player target, BasicRagdoll doll)
    {
        Player = player;
        Target = target;
        Ragdoll = doll;
        Allowed = true;
    }
}

public class Scp049RaisingEndEvent : IBaseEvent
{
    public uint EventId { get; } = ScpEvents.Scp049RaisingEnd;

    public Player Player { get; }
    public Player Target { get; }
    public BasicRagdoll Ragdoll { get; }
    public bool Allowed { get; set; }

    internal Scp049RaisingEndEvent(Player player, Player target, BasicRagdoll doll)
    {
        Player = player;
        Target = target;
        Ragdoll = doll;
        Allowed = true;
    }
}
