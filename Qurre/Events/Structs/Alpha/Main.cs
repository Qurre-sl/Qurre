using JetBrains.Annotations;
using Qurre.API;
using Qurre.API.Entities.Characters;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

/// <summary>
///     Event triggered when an Alpha warhead start sequence is initiated
/// </summary>
[PublicAPI]
public sealed class AlphaStartEvent : ICancellableEvent
{
    internal AlphaStartEvent(
        Player? player,
        bool isAutomatic,
        bool suppressSubtitles,
        AlphaWarheadSyncInfo state)
    {
        Player = player ?? Server.Host;
        IsAutomatic = isAutomatic;
        SuppressSubtitles = suppressSubtitles;
        State = state;
    }

    /// <inheritdoc />
    public uint EventId { get; } = AlphaEvents.Start;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    /// <summary>
    ///     Gets the player who initiated the warhead sequence
    /// </summary>
    public Player Player { get; }

    /// <summary>
    ///     Gets or sets whether the sequence was automatically triggered
    /// </summary>
    public bool IsAutomatic { get; set; }

    /// <summary>
    ///     Gets or sets whether to suppress CASSIE subtitles
    /// </summary>
    public bool SuppressSubtitles { get; set; }

    /// <summary>
    ///     Gets or sets the warhead state information
    /// </summary>
    public AlphaWarheadSyncInfo State { get; set; }
}

/// <summary>
///     Event triggered when Alpha warhead detonation is stopped
/// </summary>
[PublicAPI]
public sealed class AlphaStopEvent : ICancellableEvent
{
    internal AlphaStopEvent(Player? player, AlphaWarheadSyncInfo state)
    {
        Player = player ?? Server.Host;
        State = state;
    }

    /// <inheritdoc />
    public uint EventId { get; } = AlphaEvents.Stop;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    /// <summary>
    ///     Gets the player who stopped the warhead sequence
    /// </summary>
    public Player Player { get; }

    /// <summary>
    ///     Gets or sets the warhead state information
    /// </summary>
    public AlphaWarheadSyncInfo State { get; set; }
}

/// <summary>
///     Event triggered when Alpha warhead detonates
/// </summary>
[PublicAPI]
public sealed class AlphaDetonateEvent : IBaseEvent
{
    internal AlphaDetonateEvent()
    {
    }

    /// <inheritdoc />
    public uint EventId { get; } = AlphaEvents.Detonate;
}