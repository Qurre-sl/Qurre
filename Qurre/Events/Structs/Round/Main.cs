namespace Qurre.Events.Structs
{
    public class WaitingEvent : IBaseEvent
    {
        internal WaitingEvent() { }

        public uint EventId { get; } = RoundEvents.Waiting;
    }

    public class RoundStartedEvent : IBaseEvent
    {
        internal RoundStartedEvent() { }

        public uint EventId { get; } = RoundEvents.Start;
    }

    public class RoundRestartEvent : IBaseEvent
    {
        internal RoundRestartEvent() { }

        public uint EventId { get; } = RoundEvents.Restart;
    }

    public class RoundCheckEvent : IBaseEvent
    {
        internal RoundCheckEvent(LeadingTeam winner, RoundSummary.SumInfo_ClassList info, bool end)
        {
            Winner = winner;
            Info = info;
            End = end;
        }

        public uint EventId { get; } = RoundEvents.Check;

        public LeadingTeam Winner { get; set; }
        public RoundSummary.SumInfo_ClassList Info { get; set; }
        public bool End { get; set; }
    }

    public class RoundEndEvent : IBaseEvent
    {
        internal RoundEndEvent(LeadingTeam winner, RoundSummary.SumInfo_ClassList info, int toRestart)
        {
            Winner = winner;
            Info = info;
            ToRestart = toRestart;
        }

        public uint EventId { get; } = RoundEvents.End;

        public LeadingTeam Winner { get; }
        public RoundSummary.SumInfo_ClassList Info { get; set; }
        public int ToRestart { get; set; }
    }
}