// ReSharper disable once CheckNamespace

using JetBrains.Annotations;

namespace Qurre.Events.Structs;

[PublicAPI]
public class WaitingEvent : IBaseEvent
{
    internal WaitingEvent()
    {
    }

    public uint EventId { get; } = EventID;

    private const uint EventID = RoundEvents.Waiting;
}

[PublicAPI]
public class RoundStartedEvent : IBaseEvent
{
    internal RoundStartedEvent()
    {
    }

    public uint EventId { get; } = EventID;

    private const uint EventID = RoundEvents.Start;
}

[PublicAPI]
public class RoundForceStartEvent : IBaseEvent
{
    internal RoundForceStartEvent()
    {
    }

    public uint EventId { get; } = EventID;

    private const uint EventID = RoundEvents.ForceStart;
}

[PublicAPI]
public class RoundRestartEvent : IBaseEvent
{
    internal RoundRestartEvent()
    {
    }

    public uint EventId { get; } = EventID;

    private const uint EventID = RoundEvents.Restart;
}

[PublicAPI]
public class RoundCheckEvent : IBaseEvent
{
    internal RoundCheckEvent(LeadingTeam winner, RoundSummary.SumInfo_ClassList info, bool end)
    {
        Winner = winner;
        Info = info;
        End = end;
    }

    public LeadingTeam Winner { get; set; }
    public RoundSummary.SumInfo_ClassList Info { get; set; }
    public bool End { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = RoundEvents.Check;
}

[PublicAPI]
public class RoundEndEvent : IBaseEvent
{
    internal RoundEndEvent(LeadingTeam winner, RoundSummary.SumInfo_ClassList info, int toRestart)
    {
        Winner = winner;
        Info = info;
        ToRestart = toRestart;
    }

    public LeadingTeam Winner { get; }
    public RoundSummary.SumInfo_ClassList Info { get; set; }
    public int ToRestart { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = RoundEvents.End;
}