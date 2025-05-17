using JetBrains.Annotations;
using Qurre.API.Entities.Characters;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp106AttackEvent : ICancellableEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp106Attack;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    internal Scp106AttackEvent(Player attacker, Player target)
    {
        Attacker = attacker;
        Target = target;
    }
    
    #endregion

    #region Public API

    /// <inheritdoc />
    public uint EventId { get; } = EventID;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    /// <summary></summary>
    public Player Attacker { get; }

    /// <summary></summary>
    public Player Target { get; }
    
    #endregion
}