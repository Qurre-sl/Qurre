using InventorySystem;
using InventorySystem.Items.Pickups;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Controllers;

namespace Qurre.Events.Structs
{
    public class CreatePickupEvent : IBaseEvent
    {
        public uint EventId { get; } = MapEvents.CreatePickup;

        public PickupSyncInfo Info { get; }
        public Inventory Inventory { get; }
        public bool Allowed { get; set; }

        internal CreatePickupEvent(PickupSyncInfo psi, Inventory inv)
        {
            Info = psi;
            Inventory = inv;
            Allowed = true;
        }
    }

    public class RagdollSpawnEvent : IBaseEvent
    {
        public uint EventId { get; } = MapEvents.RagdollSpawn;

        public Player Owner { get; }
        public DamageHandlerBase Handler { get; }
        public bool Allowed { get; set; }

        internal RagdollSpawnEvent(Player owner, DamageHandlerBase handler)
        {
            Owner = owner ?? Server.Host;
            Handler = handler;
            Allowed = true;
        }
    }

    public class RagdollSpawnedEvent : IBaseEvent
    {
        public uint EventId { get; } = MapEvents.RagdollSpawned;

        public Ragdoll Ragdoll { get; }

        internal RagdollSpawnedEvent(Ragdoll ragdoll)
        {
            Ragdoll = ragdoll;
        }
    }
}