﻿using System.Collections.Generic;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using JetBrains.Annotations;
using NorthwoodLib.Pools;
using Qurre.API;
using Scp914;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp914UpgradeEvent : IBaseEvent
{
    internal Scp914UpgradeEvent(List<Player> players, List<ItemPickupBase> items, Vector3 moveVector, Scp914Mode mode,
        Scp914KnobSetting setting)
    {
        Players = players;
        Items = items;
        Move = moveVector;
        Mode = mode;
        Setting = setting;
        Allowed = true;
    }

    public List<Player> Players { get; }
    public List<ItemPickupBase> Items { get; }
    public Vector3 Move { get; set; }
    public Scp914Mode Mode { get; set; }
    public Scp914KnobSetting Setting { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = ScpEvents.Scp914Upgrade;
}

[PublicAPI]
public class Scp914UpgradePickupEvent : IBaseEvent
{
    internal Scp914UpgradePickupEvent(ItemPickupBase pickup, bool upgradeDropped, Vector3 moveVector,
        Scp914KnobSetting setting)
    {
        Pickup = pickup;
        UpgradeDropped = upgradeDropped;
        Setting = setting;
        Move = moveVector;
        Allowed = true;
    }

    public ItemPickupBase Pickup { get; }
    public bool UpgradeDropped { get; set; }
    public Scp914KnobSetting Setting { get; set; }
    public Vector3 Move { get; set; }

    public Vector3 TargetPosition
    {
        get => Pickup.Position + Move;
        set => Move = value - Pickup.Position;
    }

    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = ScpEvents.Scp914UpgradePickup;
}

[PublicAPI]
public class Scp914UpgradePlayerEvent : IBaseEvent
{
    internal Scp914UpgradePlayerEvent(Player player, HashSet<ItemBase>? inventory, HashSet<ItemBase>? instantUpgrade,
        bool upInventory, bool heldOnly, Vector3 moveVector, Scp914KnobSetting setting)
    {
        Player = player;
        Inventory = inventory ?? HashSetPool<ItemBase>.Shared.Rent();
        InstantUpgrade = instantUpgrade ?? HashSetPool<ItemBase>.Shared.Rent();
        UpgradeInventory = upInventory;
        HeldOnly = heldOnly;
        Setting = setting;
        Move = moveVector;
        Allowed = true;
    }

    public Player Player { get; }
    public HashSet<ItemBase> Inventory { get; }
    public HashSet<ItemBase> InstantUpgrade { get; }
    public bool UpgradeInventory { get; set; }
    public bool HeldOnly { get; set; }
    public Scp914KnobSetting Setting { get; set; }
    public Vector3 Move { get; set; }

    public Vector3 TargetPosition
    {
        get => Player.MovementState.Position + Move;
        set => Move = value - Player.MovementState.Position;
    }

    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = ScpEvents.Scp914UpgradePlayer;
}