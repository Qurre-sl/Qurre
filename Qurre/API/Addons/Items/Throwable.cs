using InventorySystem.Items.ThrowableProjectiles;
using JetBrains.Annotations;
using PlayerRoles.FirstPersonControl;
using Qurre.API.Controllers;

namespace Qurre.API.Addons.Items;

[PublicAPI]
public class Throwable(ThrowableItem itemBase) : Item(itemBase)
{
    public Throwable(ItemType itemType, Player? owner = null) : this((ThrowableItem)itemType.CreateItemInstance(owner))
    {
    }

    public ThrowableItem GameBase { get; } = itemBase;

    public ThrownProjectile Projectile => GameBase.Projectile;

    public new float Weight
    {
        get => GameBase._weight;
        set => GameBase._weight = value;
    }

    public float PinPullTime
    {
        get => GameBase._pinPullTime;
        set => GameBase._pinPullTime = value;
    }

    public void Throw(bool fullForce = true)
    {
        ThrowableItem.ProjectileSettings projectileSettings =
            fullForce ? GameBase.FullThrowSettings : GameBase.WeakThrowSettings;
        GameBase.ServerThrow(projectileSettings.StartVelocity, projectileSettings.UpwardsFactor,
            projectileSettings.StartTorque,
            ThrowableNetworkHandler.GetLimitedVelocity(Base.Owner.GetVelocity()));
    }
}