using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class WaitingEvent : IBaseEvent
{
    internal WaitingEvent()
    {
    }

    /// <inheritdoc />
    public uint EventId { get; } = RoundEvents.Waiting;
}

[PublicAPI]
public class RoundStartingEvent : ICancellableEvent
{
    internal RoundStartingEvent(bool isAllowed = true)
    {
        IsAllowed = isAllowed;
    }

    /// <inheritdoc />
    public uint EventId { get; } = RoundEvents.Starting;

    /// <inheritdoc />
    public bool IsAllowed { get; set; }
}

[PublicAPI]
public class RoundStartedEvent : IBaseEvent
{
    internal RoundStartedEvent()
    {
    }

    /// <inheritdoc />
    public uint EventId { get; } = RoundEvents.Start;
}

[PublicAPI]
public class RoundForceStartEvent : IBaseEvent
{
    internal RoundForceStartEvent()
    {
    }

    /// <inheritdoc />
    public uint EventId { get; } = RoundEvents.ForceStart;
}

[PublicAPI]
public class RoundRestartEvent : IBaseEvent
{
    internal RoundRestartEvent()
    {
    }

    /// <inheritdoc />
    public uint EventId { get; } = RoundEvents.Restart;
}

[PublicAPI]
public class RoundRestartTriggeredEvent : IBaseEvent
{
    internal RoundRestartTriggeredEvent()
    {
    }

    /// <inheritdoc />
    public uint EventId { get; } = RoundEvents.RestartTriggered;
}

[PublicAPI]
public class RoundCheckEvent : IBaseEvent
{
    internal RoundCheckEvent(RoundSummary.LeadingTeam winner, RoundSummary.SumInfo_ClassList info, bool end)
    {
        Winner = winner;
        Info = info;
        End = end;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = RoundEvents.Check;

    public RoundSummary.LeadingTeam Winner { get; set; }
    public RoundSummary.SumInfo_ClassList Info { get; set; }
    public bool End { get; set; }
}

[PublicAPI]
public class RoundEndEvent : IBaseEvent
{
    internal RoundEndEvent(RoundSummary.LeadingTeam winner, RoundSummary.SumInfo_ClassList info, int toRestart)
    {
        Winner = winner;
        Info = info;
        ToRestart = toRestart;
        ShowSummary = true;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = RoundEvents.End;

    public RoundSummary.LeadingTeam Winner { get; }
    public RoundSummary.SumInfo_ClassList Info { get; set; }
    public int ToRestart { get; set; }
    public bool ShowSummary { get; set; }
}