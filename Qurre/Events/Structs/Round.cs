using Qurre.API;
namespace Qurre.Events.Structs
{
    public class WaitingEvent : IBaseEvent
    {
        public uint EventId { get; } = RoundEvents.Waiting;

        internal WaitingEvent() { }
    }
}