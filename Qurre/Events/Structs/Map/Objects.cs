using Qurre.API;
using Qurre.API.Controllers;

namespace Qurre.Events.Structs
{
    public class TriggerTeslaEvent : IBaseEvent
    {
        public uint EventId { get; } = MapEvents.TriggerTesla;

        public Player Player { get; }
        public Tesla Tesla { get; }
        public bool InIdlingRange { get; }
        public bool InRageRange { get; }
        public bool Allowed { get; set; }

        internal TriggerTeslaEvent(Player player, Tesla tesla, bool inIdlingRange, bool inRageRange)
        {
            Player = player;
            Tesla = tesla;
            InIdlingRange = inIdlingRange;
            InRageRange = inRageRange;
            Allowed = true;
        }
    }
}