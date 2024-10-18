using Footprinting;
using InventorySystem.Items.ThrowableProjectiles;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Addons.Items;

[PublicAPI]
public sealed class GrenadeFrag : Throwable
{
    private const ItemType GrenadeFragItemType = ItemType.GrenadeHE;

    public GrenadeFrag(ThrowableItem itemBase, Player? owner = null) : base(itemBase)
    {
        ExplosionGrenade grenade = (ExplosionGrenade)GameBase.Projectile;
        MaxRadius = grenade._maxRadius;
        ScpMultiplier = grenade._scpDamageMultiplier;
        BurnDuration = grenade._burnedDuration;
        DeafenDuration = grenade._deafenedDuration;
        ConcussDuration = grenade._concussedDuration;
        FuseTime = grenade._fuseTime;
        Owner = (owner ?? itemBase.Owner.GetPlayer()) ?? Server.Host;
    }

    public GrenadeFrag() : this((ThrowableItem)GrenadeFragItemType.CreateItemInstance())
    {
    }

    public new Player Owner { get; set; }

    public float MaxRadius { get; set; }
    public float ScpMultiplier { get; set; }
    public float BurnDuration { get; set; }
    public float DeafenDuration { get; set; }
    public float ConcussDuration { get; set; }
    public float FuseTime { get; set; }

    public new void Spawn(Vector3 position, Quaternion rotation = default, Vector3 scale = default)
    {
        ExplosionGrenade grenade = (ExplosionGrenade)Object.Instantiate(GameBase.Projectile, position, rotation);
        grenade.PreviousOwner = new Footprint(Owner.ReferenceHub);
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