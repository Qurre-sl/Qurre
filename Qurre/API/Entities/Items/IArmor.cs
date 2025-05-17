using InventorySystem.Items.Armor;
using JetBrains.Annotations;
using Qurre.API.Core;

namespace Qurre.API.Entities.Items;

[PublicAPI]
public interface IArmor : IItem
{
    new UnityObjectWrapper<BodyArmor> Base { get; }

    bool IsEquippable { get; }

    bool IsHolsterable { get; }

    bool IsWorn { get; }

    /*bool RemoveExcessOnDrop { get; set; }*/ // TODO: removed in 14.1

    int HelmetEfficacy { get; set; }

    int VestEfficacy { get; set; }

    float StaminaUsageMultiplier { get; set; }

    float MovementSpeedMultiplier { get; set; }

    float CivilianDownsideMultiplier { get; set; }

    BodyArmor.ArmorAmmoLimit[] AmmoLimits { get; set; }
}