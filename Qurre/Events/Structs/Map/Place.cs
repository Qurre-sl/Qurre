using InventorySystem;
using InventorySystem.Items.Pickups;
using JetBrains.Annotations;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class CreatePickupEvent : IBaseEvent
{
    internal CreatePickupEvent(PickupSyncInfo psi, Inventory inv)
    {
        Info = psi;
        Inventory = inv;
        Allowed = true;
    }

    public PickupSyncInfo Info { get; }
    public Inventory Inventory { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = MapEvents.CreatePickup;
}

[PublicAPI]
public class RagdollSpawnEvent : IBaseEvent
{
    internal RagdollSpawnEvent(Player owner, DamageHandlerBase handler)
    {
        Owner = owner ?? Server.Host;
        Handler = handler;
        Allowed = true;
    }

    public Player Owner { get; }
    public DamageHandlerBase Handler { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = MapEvents.RagdollSpawn;
}

[PublicAPI]
public class RagdollSpawnedEvent : IBaseEvent
{
    internal RagdollSpawnedEvent(Ragdoll ragdoll)
    {
        Ragdoll = ragdoll;
    }

    public Ragdoll Ragdoll { get; }
    public uint EventId { get; } = EventID;

    private const uint EventID = MapEvents.RagdollSpawned;
}