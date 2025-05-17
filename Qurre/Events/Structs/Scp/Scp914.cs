using System;
using System.Collections.Generic;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using JetBrains.Annotations;
using NorthwoodLib.Pools;
using Qurre.API.Controllers;
using Scp914;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp914UpgradeEvent : IBaseEvent
{
    private const uint EventID = ScpEvents.Scp914Upgrade;

    internal Scp914UpgradeEvent(List<Player> players, List<ItemPickupBase> items, Scp914Mode mode,
        Scp914KnobSetting setting)
    {
        Players = players;
        Items = items;
        Mode = mode;
        Setting = setting;
        Allowed = true;
    }

    public List<Player> Players { get; }
    public List<ItemPickupBase> Items { get; }
    public Scp914Mode Mode { get; set; }
    public Scp914KnobSetting Setting { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class Scp914UpgradePickupEvent : IBaseEvent
{
    private const uint EventID = ScpEvents.Scp914UpgradePickup;

    internal Scp914UpgradePickupEvent(ItemPickupBase pickup, bool upgradeDropped, Vector3 moveVector,
        Scp914KnobSetting setting)
    {
        Pickup = pickup;
        UpgradeDropped = upgradeDropped;
        Setting = setting;
        // Move = moveVector; todo
        Allowed = true;
    }

    public ItemPickupBase Pickup { get; }
    public bool UpgradeDropped { get; set; }
    public Scp914KnobSetting Setting { get; set; }

    public Vector3 Move
    {
        get => Scp914Controller.MoveVector;
        // todo: update later
        [Obsolete("Outdated in v14")]
        // ReSharper disable once ValueParameterNotUsed
        set { }
    }

    public Vector3 TargetPosition
    {
        get => Pickup.Position + Move;
        // todo: update later
        [Obsolete("Outdated in v14")]
        set => Move = value - Pickup.Position;
    }

    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class Scp914UpgradePlayerEvent : IBaseEvent
{
    private const uint EventID = ScpEvents.Scp914UpgradePlayer;

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
        Allowed = true;
    }

    public Player Player { get; }
    public HashSet<ItemBase> Inventory { get; }
    public HashSet<ItemBase> InstantUpgrade { get; }
    public bool UpgradeInventory { get; set; }
    public bool HeldOnly { get; set; }
    public Scp914KnobSetting Setting { get; set; }

    public Vector3 Move
    {
        get => Scp914Controller.MoveVector;
        // todo: update later
        [Obsolete("Outdated in v14")]
        // ReSharper disable once ValueParameterNotUsed
        set { }
    }

    public Vector3 TargetPosition
    {
        get => Player.MovementState.Position + Move;
        // todo: update later
        [Obsolete("Outdated in v14")]
        set => Move = value - Player.MovementState.Position;
    }

    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}