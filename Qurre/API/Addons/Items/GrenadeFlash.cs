using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using UnityEngine;

namespace Qurre.API.Addons.Items
{
    public sealed class GrenadeFlash : Throwable
    {
        private const ItemType GrenadeFlashItemType = ItemType.GrenadeFlash;

        private Player _owner;

        public GrenadeFlash(ThrowableItem itemBase, Player owner = null) : base(itemBase)
        {
            var grenade = (FlashbangGrenade)Base.Projectile;
            BlindAnimation = grenade._blindingOverDistance;
            SurfaceDistanceIntensifier = grenade._surfaceZoneDistanceIntensifier;
            DeafenAnimation = grenade._deafenDurationOverDistance;
            FuseTime = grenade._fuseTime;
            _owner = (owner ?? itemBase.Owner.GetPlayer()) ?? Server.Host;
        }

        public GrenadeFlash(ThrowableItem itemBase) : this(itemBase, null) { }

        public GrenadeFlash() : this((ThrowableItem)GrenadeFlashItemType.CreateItemInstance()) { }

        public new Player Owner
        {
            get => _owner ?? Server.Host;
            set => _owner = value;
        }

        public AnimationCurve BlindAnimation { get; set; }
        public float SurfaceDistanceIntensifier { get; set; }
        public AnimationCurve DeafenAnimation { get; set; }
        public float FuseTime { get; set; }

        public new void Spawn(Vector3 position, Quaternion rotation = default, Vector3 scale = default)
        {
            var grenade = (FlashbangGrenade)Object.Instantiate(Base.Projectile, position, rotation);
            grenade.PreviousOwner = new (Owner.ReferenceHub);
            grenade._blindingOverDistance = BlindAnimation;
            grenade._surfaceZoneDistanceIntensifier = SurfaceDistanceIntensifier;
            grenade._deafenDurationOverDistance = DeafenAnimation;
            grenade._fuseTime = FuseTime;

            if (scale != default)
            {
                grenade.transform.localScale = scale;
            }

            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();
        }
    }
}