using InventorySystem;
using InventorySystem.Items.Pickups;
using JetBrains.Annotations;
using PlayerStatsSystem;
using Qurre.API.Entities.Characters;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class CreatePickupEvent : ICancellableEvent
{
    internal CreatePickupEvent(PickupSyncInfo psi, Inventory inv)
    {
        Info = psi;
        Inventory = inv;
    }

    /// <inheritdoc />
    public uint EventId { get; } = MapEvents.CreatePickup;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public PickupSyncInfo Info { get; }
    public Inventory Inventory { get; }
}

[PublicAPI]
public class CorpseSpawnEvent : ICancellableEvent
{
    internal CorpseSpawnEvent(Player owner, DamageHandlerBase handler)
    {
        Owner = owner;
        Handler = handler;
    }

    /// <inheritdoc />
    public uint EventId { get; } = MapEvents.CorpseSpawn;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Owner { get; }
    public DamageHandlerBase Handler { get; }
}

[PublicAPI]
public class CorpseSpawnedEvent : IBaseEvent
{
    internal CorpseSpawnedEvent(ICorpse corpse)
    {
        Corpse = corpse;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = MapEvents.CorpseSpawned;

    public ICorpse Corpse { get; }
}