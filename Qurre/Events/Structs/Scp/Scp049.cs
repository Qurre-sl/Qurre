using JetBrains.Annotations;
using PlayerRoles.Ragdolls;
using Qurre.API;
using Qurre.API.Entities;
using Qurre.API.Entities.Characters;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp049RaisingStartEvent : ICancellableEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp049RaisingStart;

    #endregion

    #region Constructor

    internal Scp049RaisingStartEvent(Player issuer, Player target, BasicRagdoll basicRagdoll)
    {
        Issuer = issuer;
        Target = target;
        Corpse = basicRagdoll.GetCorpse()!;
    }
    
    #endregion
    
    #region Public API

    /// <inheritdoc />
    public uint EventId { get; } = ScpEvents.Scp049RaisingStart;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;
    
    public Player Issuer { get; }
    
    public Player Target { get; }
    
    public ICorpse Corpse { get; }
    
    #endregion
}

[PublicAPI]
public class Scp049RaisingEndEvent : ICancellableEvent
{
    internal Scp049RaisingEndEvent(Player issuer, Player target, BasicRagdoll basicRagdoll)
    {
        Issuer = issuer;
        Target = target;
        Corpse = EntityManager.GetOrException<ICorpse>(basicRagdoll);
    }

    /// <inheritdoc />
    public uint EventId { get; } = ScpEvents.Scp049RaisingEnd;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;
    
    public Player Issuer { get; }
    public Player Target { get; }
    public ICorpse Corpse { get; }

}