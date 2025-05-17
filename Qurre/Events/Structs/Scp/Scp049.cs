using JetBrains.Annotations;
using PlayerRoles.Ragdolls;
using Qurre.API;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp049RaisingStartEvent : IBaseEvent
{
    private const uint EventID = ScpEvents.Scp049RaisingStart;

    internal Scp049RaisingStartEvent(Player player, Player target, BasicRagdoll doll)
    {
        Player = player;
        Target = target;
        Corpse = doll.GetCorpse();
        Allowed = true;
    }

    public Player Player { get; }
    public Player Target { get; }
    public Corpse Corpse { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class Scp049RaisingEndEvent : IBaseEvent
{
    private const uint EventID = ScpEvents.Scp049RaisingEnd;

    internal Scp049RaisingEndEvent(Player player, Player target, BasicRagdoll doll)
    {
        Player = player;
        Target = target;
        Corpse = doll.GetCorpse();
        Allowed = true;
    }

    public Player Player { get; }
    public Player Target { get; }
    public Corpse Corpse { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}