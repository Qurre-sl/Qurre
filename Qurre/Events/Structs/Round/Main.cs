namespace Qurre.Events.Structs
{
    public class WaitingEvent : IBaseEvent
    {
        public uint EventId { get; } = RoundEvents.Waiting;

        internal WaitingEvent() { }
    }

    public class RoundStartedEvent : IBaseEvent
    {
        public uint EventId { get; } = RoundEvents.Start;

        internal RoundStartedEvent() { }
    }

    public class RoundRestartEvent : IBaseEvent
    {
        public uint EventId { get; } = RoundEvents.Restart;

        internal RoundRestartEvent() { }
    }

    public class RoundCheckEvent : IBaseEvent
    {
        public uint EventId { get; } = RoundEvents.Check;

        public LeadingTeam Winner { get; set; }
        public RoundSummary.SumInfo_ClassList Info { get; set; }
        public bool End { get; set; }

        internal RoundCheckEvent(LeadingTeam winner, RoundSummary.SumInfo_ClassList info, bool end)
        {
            Winner = winner;
            Info = info;
            End = end;
        }
    }

    public class RoundEndEvent : IBaseEvent
    {
        public uint EventId { get; } = RoundEvents.End;

        public LeadingTeam Winner { get; }
        public RoundSummary.SumInfo_ClassList Info { get; set; }
        public int ToRestart { get; set; }

        internal RoundEndEvent(LeadingTeam winner, RoundSummary.SumInfo_ClassList info, int toRestart)
        {
            Winner = winner;
            Info = info;
            ToRestart = toRestart;
        }
    }
}