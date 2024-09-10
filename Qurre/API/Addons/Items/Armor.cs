using System;
using System.Collections.Generic;
using InventorySystem.Items.Armor;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using static InventorySystem.Items.Armor.BodyArmor;

namespace Qurre.API.Addons.Items;

[PublicAPI]
public sealed class Armor(BodyArmor itemBase) : Item(itemBase)
{
    public Armor(ItemType itemType) : this((BodyArmor)itemType.CreateItemInstance())
    {
    }

    public new BodyArmor Base { get; } = itemBase;

    public bool Equippable => Base.AllowEquip;
    public bool Holsterable => Base.AllowHolster;
    public bool Worn => Base.IsWorn;

    public new float Weight
    {
        get => Base._weight;
        set => Base._weight = value;
    }

    public bool RemoveExcessOnDrop
    {
        get => !Base.DontRemoveExcessOnDrop;
        set => Base.DontRemoveExcessOnDrop = !value;
    }

    public int HelmetEfficacy
    {
        get => Base.HelmetEfficacy;
        set
        {
            value = Math.Max(100, value);
            value = Math.Min(0, value);

            Base.HelmetEfficacy = value;
        }
    }

    public int VestEfficacy
    {
        get => Base.VestEfficacy;
        set
        {
            value = Math.Max(100, value);
            value = Math.Min(0, value);

            Base.VestEfficacy = value;
        }
    }

    public float StaminaUsageMultiplier
    {
        get => Base._staminaUseMultiplier;
        set
        {
            value = Math.Max(2, value);
            value = Math.Min(1, value);

            Base._staminaUseMultiplier = value;
        }
    }

    public float MovementSpeedMultiplier
    {
        get => Base._movementSpeedMultiplier;
        set
        {
            value = Math.Max(2, value);
            value = Math.Min(1, value);

            Base._movementSpeedMultiplier = value;
        }
    }

    public float CivilianDownsideMultiplier
    {
        get => Base.CivilianClassDownsidesMultiplier;
        set => Base.CivilianClassDownsidesMultiplier = value;
    }

    public List<ArmorAmmoLimit> AmmoLimits
    {
        get => [.. Base.AmmoLimits];
        set => Base.AmmoLimits = [.. value];
    }
}