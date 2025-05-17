// ReSharper disable once CheckNamespace

using JetBrains.Annotations;

namespace Qurre.Events.Structs;

[PublicAPI]
public class WaitingEvent : IBaseEvent
{
    private const uint EventID = RoundEvents.Waiting;

    internal WaitingEvent()
    {
    }

    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class RoundStartedEvent : IBaseEvent
{
    private const uint EventID = RoundEvents.Start;

    internal RoundStartedEvent()
    {
    }

    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class RoundForceStartEvent : IBaseEvent
{
    private const uint EventID = RoundEvents.ForceStart;

    internal RoundForceStartEvent()
    {
    }

    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class RoundRestartEvent : IBaseEvent
{
    private const uint EventID = RoundEvents.Restart;

    internal RoundRestartEvent()
    {
    }

    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class RoundCheckEvent : IBaseEvent
{
    private const uint EventID = RoundEvents.Check;

    internal RoundCheckEvent(RoundSummary.LeadingTeam winner, RoundSummary.SumInfo_ClassList info, bool end)
    {
        Winner = winner;
        Info = info;
        End = end;
    }

    public RoundSummary.LeadingTeam Winner { get; set; }
    public RoundSummary.SumInfo_ClassList Info { get; set; }
    public bool End { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class RoundEndEvent : IBaseEvent
{
    private const uint EventID = RoundEvents.End;

    internal RoundEndEvent(RoundSummary.LeadingTeam winner, RoundSummary.SumInfo_ClassList info, int toRestart)
    {
        Winner = winner;
        Info = info;
        ToRestart = toRestart;
    }

    public RoundSummary.LeadingTeam Winner { get; }
    public RoundSummary.SumInfo_ClassList Info { get; set; }
    public int ToRestart { get; set; }
    public uint EventId { get; } = EventID;
}