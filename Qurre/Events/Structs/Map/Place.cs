using InventorySystem;
using InventorySystem.Items.Pickups;
using JetBrains.Annotations;
using PlayerStatsSystem;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class CreatePickupEvent : IBaseEvent
{
    private const uint EventID = MapEvents.CreatePickup;

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
}

[PublicAPI]
public class CorpseSpawnEvent : IBaseEvent
{
    private const uint EventID = MapEvents.CorpseSpawn;

    internal CorpseSpawnEvent(Player owner, DamageHandlerBase handler)
    {
        Owner = owner;
        Handler = handler;
        Allowed = true;
    }

    public Player Owner { get; }
    public DamageHandlerBase Handler { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class CorpseSpawnedEvent : IBaseEvent
{
    private const uint EventID = MapEvents.CorpseSpawned;

    internal CorpseSpawnedEvent(Corpse corpse)
    {
        Corpse = corpse;
    }

    public Corpse Corpse { get; }
    public uint EventId { get; } = EventID;
}