using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Addons.Items
{
    public sealed class GrenadeFrag : Throwable
    {
        private const ItemType GrenadeFragItemType = ItemType.GrenadeHE;

        public new Player Owner
        {
            get => _owner ?? Server.Host;
            set => _owner = value;
        }

        public float MaxRadius { get; set; }
        public float ScpMultiplier { get; set; }
        public float BurnDuration { get; set; }
        public float DeafenDuration { get; set; }
        public float ConcussDuration { get; set; }
        public float FuseTime { get; set; }

        private Player _owner;

        public GrenadeFrag(ThrowableItem itemBase, Player owner = null) : base(itemBase)
        {
            ExplosionGrenade grenade = (ExplosionGrenade)Base.Projectile;
            MaxRadius = grenade._maxRadius;
            ScpMultiplier = grenade._scpDamageMultiplier;
            BurnDuration = grenade._burnedDuration;
            DeafenDuration = grenade._deafenedDuration;
            ConcussDuration = grenade._concussedDuration;
            FuseTime = grenade._fuseTime;
            _owner = (owner ?? itemBase.Owner.GetPlayer()) ?? Server.Host;
        }

        public GrenadeFrag(ThrowableItem itemBase) : this(itemBase, null)
        {
        }

        public GrenadeFrag() : this((ThrowableItem)GrenadeFragItemType.CreateItemInstance())
        {
        }

        public new void Spawn(Vector3 position, Quaternion rotation = default, Vector3 scale = default)
        {
            ExplosionGrenade grenade = (ExplosionGrenade)Object.Instantiate(Base.Projectile, position, rotation);
            grenade.PreviousOwner = new Footprinting.Footprint(Owner.ReferenceHub);
            grenade._maxRadius = MaxRadius;
            grenade._scpDamageMultiplier = ScpMultiplier;
            grenade._burnedDuration = BurnDuration;
            grenade._deafenedDuration = DeafenDuration;
            grenade._concussedDuration = ConcussDuration;
            grenade._fuseTime = FuseTime;

            if (scale != default)
                grenade.transform.localScale = scale;

            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();
        }
    }
}