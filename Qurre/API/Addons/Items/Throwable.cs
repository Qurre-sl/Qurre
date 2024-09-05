using InventorySystem.Items.ThrowableProjectiles;
using Qurre.API.Controllers;
using PlayerRoles.FirstPersonControl;

namespace Qurre.API.Addons.Items
{
    public class Throwable : Item
    {
        public new ThrowableItem Base { get; }

        public ThrownProjectile Projectile => Base.Projectile;

        public new float Weight
        {
            get => Base._weight;
            set => Base._weight = value;
        }

        public float PinPullTime
        {
            get => Base._pinPullTime;
            set => Base._pinPullTime = value;
        }

        public Throwable(ThrowableItem itemBase) : base(itemBase)
        {
            Base = itemBase;
        }

        public Throwable(ItemType itemType, Player owner = null) : this((ThrowableItem)itemType.CreateItemInstance(owner))
        {
        }

        public void Throw(bool fullForce = true)
        {
            ThrowableItem.ProjectileSettings projectileSettings = fullForce ? Base.FullThrowSettings : Base.WeakThrowSettings;
            Base.ServerThrow(projectileSettings.StartVelocity, projectileSettings.UpwardsFactor, projectileSettings.StartTorque,
                ThrowableNetworkHandler.GetLimitedVelocity(Base.Owner.GetVelocity()));
        }
    }
}