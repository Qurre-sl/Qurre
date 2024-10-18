using InventorySystem.Items.Usables;
using JetBrains.Annotations;
using Qurre.API.Controllers;

namespace Qurre.API.Addons.Items;

[PublicAPI]
public sealed class Usable(UsableItem itemBase) : Item(itemBase)
{
    public Usable(ItemType type) : this((UsableItem)type.CreateItemInstance())
    {
    }

    public UsableItem GameBase { get; } = itemBase;

    public bool Equippable => GameBase.AllowEquip;
    public bool Holsterable => GameBase.AllowHolster;

    public new float Weight
    {
        get => GameBase._weight;
        set => GameBase._weight = value;
    }

    public float UseTime
    {
        get => GameBase.UseTime;
        set => GameBase.UseTime = value;
    }

    public float MaxCancellableTime
    {
        get => GameBase.MaxCancellableTime;
        set => GameBase.MaxCancellableTime = value;
    }

    public float RemainingCooldown
    {
        get => GameBase.RemainingCooldown;
        set => GameBase.RemainingCooldown = value;
    }
}