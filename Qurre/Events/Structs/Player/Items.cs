using InventorySystem.Items.Radio;
using JetBrains.Annotations;
using Qurre.API.Entities;
using Qurre.API.Entities.Characters;
using Qurre.API.Entities.Items;
using Qurre.API.Enums;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class CancelUseItemEvent : ICancellableEvent
{
    internal CancelUseItemEvent(Player player, IItem item)
    {
        Player = player;
        Item = item;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.CancelUseItem;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IItem Item { get; }
}

[PublicAPI]
public class UseItemEvent : ICancellableEvent
{
    internal UseItemEvent(Player player, IItem item)
    {
        Player = player;
        Item = item;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.UseItem;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IItem Item { get; }
}

[PublicAPI]
public class UsedItemEvent : IBaseEvent
{
    internal UsedItemEvent(Player player, IItem item)
    {
        Player = player;
        Item = item;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.UsedItem;

    public Player Player { get; }
    public IItem Item { get; }
}

[PublicAPI]
public class ChangeItemEvent : ICancellableEvent
{
    internal ChangeItemEvent(Player player, IItem? oldItem, IItem? newItem)
    {
        Player = player;
        OldItem = oldItem;
        NewItem = newItem;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.ChangeItem;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IItem? OldItem { get; }
    public IItem? NewItem { get; }
}

[PublicAPI]
public class UpdateRadioEvent : ICancellableEvent
{
    internal UpdateRadioEvent(Player player, RadioItem radioBase, RadioStatus range, bool enabled)
    {
        Player = player;
        Radio = EntityManager.GetOrException<IRadio>(radioBase);
        Range = range;
        Enabled = enabled;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.UpdateRadio;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IRadio Radio { get; }
    public RadioStatus Range { get; set; }
    public bool Enabled { get; set; }
}

[PublicAPI]
public class UsingRadioEvent : ICancellableEvent
{
    internal UsingRadioEvent(Player player, RadioItem radioBase, float num)
    {
        Player = player;
        Radio = EntityManager.GetOrException<IRadio>(radioBase);
        Battery = radioBase._battery * 100;
        Consumption = Time.deltaTime * (num / 60 / 100) * 100;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.UsingRadio;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IRadio Radio { get; }
    public float Battery { get; set; }
    public float Consumption { get; set; }
}