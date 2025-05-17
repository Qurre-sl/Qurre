using Footprinting;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using Qurre.API.Entities.Characters;
using Qurre.Internal.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Entities.Items.Implementations;

[EntityWrapBindForFactory(typeof(ThrowableItem))]
internal sealed class GrenadeFrag : Throwable, IGrenadeFrag
{
    public GrenadeFrag(ThrowableItem itemBase, Player? owner = null) : base(itemBase)
    {
        var grenade = (ExplosionGrenade)Base.Instance.Projectile;
        MaxRadius = grenade.MaxRadius;
        ScpMultiplier = grenade.ScpDamageMultiplier;
        BurnDuration = grenade._burnedDuration;
        DeafenDuration = grenade._deafenedDuration;
        ConcussDuration = grenade._concussedDuration;
        FuseTime = grenade._fuseTime;
    }

    public float MaxRadius { get; set; }
    public float ScpMultiplier { get; set; }
    public float BurnDuration { get; set; }
    public float DeafenDuration { get; set; }
    public float ConcussDuration { get; set; }
    public float FuseTime { get; set; }

    public new void Spawn(Vector3 position, Quaternion rotation = default, Vector3 scale = default)
    {
        var grenade = (ExplosionGrenade)Object.Instantiate(Base.Instance.Projectile, position, rotation);
        grenade.PreviousOwner = new Footprint(Owner.ReferenceHub);
        grenade.MaxRadius = MaxRadius;
        grenade.ScpDamageMultiplier = ScpMultiplier;
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