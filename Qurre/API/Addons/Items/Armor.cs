using InventorySystem.Items.Armor;
using Qurre.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using static InventorySystem.Items.Armor.BodyArmor;

namespace Qurre.API.Addons.Items
{
    public sealed class Armor : Item
    {
        public new BodyArmor Base { get; }

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

                Base.HelmetEfficacy = value;
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
            get => Base.AmmoLimits.ToList();
            set => Base.AmmoLimits = value.ToArray();
        }

        public Armor(BodyArmor itemBase) : base(itemBase)
        {
            Base = itemBase;
        }

        public Armor(ItemType itemType) : this((BodyArmor)itemType.CreateItemInstance())
        {
        }
    }
}