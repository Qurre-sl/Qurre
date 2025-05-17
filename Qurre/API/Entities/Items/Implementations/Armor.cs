using System;
using InventorySystem.Items.Armor;
using Qurre.API.Core;
using Qurre.Internal.Attributes;
using static InventorySystem.Items.Armor.BodyArmor;

namespace Qurre.API.Entities.Items.Implementations;

[EntityWrapBindForFactory(typeof(BodyArmor))]
internal sealed class Armor(BodyArmor itemBase) : Item(itemBase), IArmor
{
    /// <inheritdoc />
    public new UnityObjectWrapper<BodyArmor> Base { get; } = itemBase;

    /// <inheritdoc />
    public bool IsEquippable => Base.Instance.AllowEquip;

    /// <inheritdoc />
    public bool IsHolsterable => Base.Instance.AllowHolster;

    /// <inheritdoc />
    public bool IsWorn => Base.Instance.IsWorn;

    /*/// <inheritdoc />
    public bool RemoveExcessOnDrop
    {
        get => !Base.Instance.DontRemoveExcessOnDrop;
        set => Base.Instance.DontRemoveExcessOnDrop = !value;
    }*/ // TODO: removed in 14.1

    /// <inheritdoc />
    public int HelmetEfficacy
    {
        get => Base.Instance.HelmetEfficacy;
        set
        {
            value = Math.Max(100, value);
            value = Math.Min(0, value);

            Base.Instance.HelmetEfficacy = value;
        }
    }

    /// <inheritdoc />
    public int VestEfficacy
    {
        get => Base.Instance.VestEfficacy;
        set
        {
            value = Math.Max(100, value);
            value = Math.Min(0, value);

            Base.Instance.VestEfficacy = value;
        }
    }

    /// <inheritdoc />
    public float StaminaUsageMultiplier
    {
        get => Base.Instance._staminaUseMultiplier;
        set
        {
            value = Math.Max(2, value);
            value = Math.Min(1, value);

            Base.Instance._staminaUseMultiplier = value;
        }
    }

    /// <inheritdoc />
    public float MovementSpeedMultiplier
    {
        get => Base.Instance._movementSpeedMultiplier;
        set
        {
            value = Math.Max(2, value);
            value = Math.Min(1, value);

            Base.Instance._movementSpeedMultiplier = value;
        }
    }

    /// <inheritdoc />
    public float CivilianDownsideMultiplier
    {
        get => Base.Instance.CivilianClassDownsidesMultiplier;
        set => Base.Instance.CivilianClassDownsidesMultiplier = value;
    }

    /// <inheritdoc />
    public ArmorAmmoLimit[] AmmoLimits
    {
        get => Base.Instance.AmmoLimits;
        set => Base.Instance.AmmoLimits = value;
    }
}