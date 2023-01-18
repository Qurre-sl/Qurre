using PlayerStatsSystem;
using Qurre.API;

namespace Qurre.Events.Structs
{
    public class RagdollSpawnEvent : IBaseEvent
    {
        internal RagdollSpawnEvent(Player owner, DamageHandlerBase handler)
        {
            Owner = owner ?? Server.Host;
            Handler = handler;
            Allowed = true;
        }

        public uint EventId { get; } = MapEvents.RagdollSpawn;

        public Player Owner { get; }
        public DamageHandlerBase Handler { get; }
        public bool Allowed { get; set; }
    }
}