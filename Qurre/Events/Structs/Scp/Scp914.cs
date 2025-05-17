using System;
using System.Collections.Generic;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using JetBrains.Annotations;
using NorthwoodLib.Pools;
using Qurre.API.Entities.Characters;
using Scp914;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp914UpgradeEvent : ICancellableEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp914Upgrade;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="players"></param>
    /// <param name="items"></param>
    /// <param name="mode"></param>
    /// <param name="setting"></param>
    internal Scp914UpgradeEvent(List<Player> players, List<ItemPickupBase> items, Scp914Mode mode,
        Scp914KnobSetting setting)
    {
        Players = players;
        Items = items;
        Mode = mode;
        Setting = setting;
    }

    #endregion

    #region Public API

    /// <inheritdoc />
    public uint EventId { get; } = EventID;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    /// <summary></summary>
    public List<Player> Players { get; }
    
    /// <summary></summary>
    public List<ItemPickupBase> Items { get; }
    
    /// <summary></summary>
    public Scp914Mode Mode { get; set; }
    
    /// <summary></summary>
    public Scp914KnobSetting Setting { get; set; }
    
    #endregion
}

[PublicAPI]
public class Scp914UpgradePickupEvent : ICancellableEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp914UpgradePickup;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pickup"></param>
    /// <param name="upgradeDropped"></param>
    /// <param name="moveVector"></param>
    /// <param name="setting"></param>
    internal Scp914UpgradePickupEvent(ItemPickupBase pickup, bool upgradeDropped, Vector3 moveVector,
        Scp914KnobSetting setting)
    {
        Pickup = pickup;
        UpgradeDropped = upgradeDropped;
        Setting = setting;
        // Move = moveVector; todo
    }
    
    #endregion

    #region Public API

    /// <inheritdoc />
    public uint EventId { get; } = EventID;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;
    
    /// <summary></summary>
    public ItemPickupBase Pickup { get; }
    
    /// <summary></summary>
    public bool UpgradeDropped { get; set; }
    
    /// <summary></summary>
    public Scp914KnobSetting Setting { get; set; }

    /// <summary></summary>
    public Vector3 Move
    {
        get => Scp914Controller.MoveVector;
        // todo: update later
        [Obsolete("Outdated in v14")]
        // ReSharper disable once ValueParameterNotUsed
        set { }
    }

    /// <summary></summary>
    public Vector3 TargetPosition
    {
        get => Pickup.Position + Move;
        // todo: update later
        [Obsolete("Outdated in v14")]
        set => Move = value - Pickup.Position;
    }
    
    #endregion
}

[PublicAPI]
public class Scp914UpgradePlayerEvent : ICancellableEvent
{
    #region Constants

    // Unique identifier of the event (do not rename or alter).
    private const uint EventID = ScpEvents.Scp914UpgradePlayer;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="inventory"></param>
    /// <param name="instantUpgrade"></param>
    /// <param name="upInventory"></param>
    /// <param name="heldOnly"></param>
    /// <param name="moveVector"></param>
    /// <param name="setting"></param>
    internal Scp914UpgradePlayerEvent(Player player, HashSet<ItemBase>? inventory, HashSet<ItemBase>? instantUpgrade,
        bool upInventory, bool heldOnly, Vector3 moveVector, Scp914KnobSetting setting)
    {
        Player = player;
        Inventory = inventory ?? HashSetPool<ItemBase>.Shared.Rent();
        InstantUpgrade = instantUpgrade ?? HashSetPool<ItemBase>.Shared.Rent();
        UpgradeInventory = upInventory;
        HeldOnly = heldOnly;
        Setting = setting;
        // Move = moveVector; todo
    }
    
    #endregion

    #region Public API

    /// <inheritdoc />
    public uint EventId { get; } = EventID;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    /// <summary></summary>
    public Player Player { get; }
    
    /// <summary></summary>
    public HashSet<ItemBase> Inventory { get; }
    
    /// <summary></summary>
    public HashSet<ItemBase> InstantUpgrade { get; }
    
    /// <summary></summary>
    public bool UpgradeInventory { get; set; }
    
    /// <summary></summary>
    public bool HeldOnly { get; set; }
    
    /// <summary></summary>
    public Scp914KnobSetting Setting { get; set; }

    /// <summary></summary>
    public Vector3 Move
    {
        get => Scp914Controller.MoveVector;
        // todo: update later
        [Obsolete("Outdatednd in v14")]
        // ReSharper disable once ValueParameterNotUsed
        set { }
    }

    /// <summary></summary>
    public Vector3 TargetPosition
    {
        get => Player.MovementState.Position + Move;
        // todo: update later
        [Obsolete("Outdated in v14")]
        set => Move = value - Player.MovementState.Position;
    }
    
    #endregion
}