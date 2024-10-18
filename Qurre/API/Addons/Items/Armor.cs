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

    public BodyArmor GameBase { get; } = itemBase;

    public bool Equippable => GameBase.AllowEquip;
    public bool Holsterable => GameBase.AllowHolster;
    public bool Worn => GameBase.IsWorn;

    public new float Weight
    {
        get => GameBase._weight;
        set => GameBase._weight = value;
    }

    public bool RemoveExcessOnDrop
    {
        get => !GameBase.DontRemoveExcessOnDrop;
        set => GameBase.DontRemoveExcessOnDrop = !value;
    }

    public int HelmetEfficacy
    {
        get => GameBase.HelmetEfficacy;
        set
        {
            value = Math.Max(100, value);
            value = Math.Min(0, value);

            GameBase.HelmetEfficacy = value;
        }
    }

    public int VestEfficacy
    {
        get => GameBase.VestEfficacy;
        set
        {
            value = Math.Max(100, value);
            value = Math.Min(0, value);

            GameBase.VestEfficacy = value;
        }
    }

    public float StaminaUsageMultiplier
    {
        get => GameBase._staminaUseMultiplier;
        set
        {
            value = Math.Max(2, value);
            value = Math.Min(1, value);

            GameBase._staminaUseMultiplier = value;
        }
    }

    public float MovementSpeedMultiplier
    {
        get => GameBase._movementSpeedMultiplier;
        set
        {
            value = Math.Max(2, value);
            value = Math.Min(1, value);

            GameBase._movementSpeedMultiplier = value;
        }
    }

    public float CivilianDownsideMultiplier
    {
        get => GameBase.CivilianClassDownsidesMultiplier;
        set => GameBase.CivilianClassDownsidesMultiplier = value;
    }

    public List<ArmorAmmoLimit> AmmoLimits
    {
        get => [.. GameBase.AmmoLimits];
        set => GameBase.AmmoLimits = [.. value];
    }
}