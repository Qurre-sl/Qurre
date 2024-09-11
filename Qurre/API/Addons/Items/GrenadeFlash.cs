using Footprinting;
using InventorySystem.Items.ThrowableProjectiles;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Addons.Items;

[PublicAPI]
public sealed class GrenadeFlash : Throwable
{
    private const ItemType GrenadeFlashItemType = ItemType.GrenadeFlash;

    public GrenadeFlash(ThrowableItem itemBase, Player? owner = null) : base(itemBase)
    {
        FlashbangGrenade grenade = (FlashbangGrenade)Base.Projectile;
        BlindAnimation = grenade._blindingOverDistance;
        SurfaceDistanceIntensifier = grenade._surfaceZoneDistanceIntensifier;
        DeafenAnimation = grenade._deafenDurationOverDistance;
        FuseTime = grenade._fuseTime;
        Owner = (owner ?? itemBase.Owner.GetPlayer()) ?? Server.Host;
    }

    public GrenadeFlash() : this((ThrowableItem)GrenadeFlashItemType.CreateItemInstance())
    {
    }

    public new Player Owner { get; set; }

    public AnimationCurve BlindAnimation { get; set; }
    public float SurfaceDistanceIntensifier { get; set; }
    public AnimationCurve DeafenAnimation { get; set; }
    public float FuseTime { get; set; }

    public new void Spawn(Vector3 position, Quaternion rotation = default, Vector3 scale = default)
    {
        FlashbangGrenade grenade = (FlashbangGrenade)Object.Instantiate(Base.Projectile, position, rotation);
        grenade.PreviousOwner = new Footprint(Owner.ReferenceHub);
        grenade._blindingOverDistance = BlindAnimation;
        grenade._surfaceZoneDistanceIntensifier = SurfaceDistanceIntensifier;
        grenade._deafenDurationOverDistance = DeafenAnimation;
        grenade._fuseTime = FuseTime;

        if (scale != default)
            grenade.transform.localScale = scale;

        NetworkServer.Spawn(grenade.gameObject);
        grenade.ServerActivate();
    }
}