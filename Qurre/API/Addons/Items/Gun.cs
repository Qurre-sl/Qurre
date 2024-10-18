using System;
using CameraShaking;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Attachments.Components;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using Qurre.API.Objects;

namespace Qurre.API.Addons.Items;

[PublicAPI]
public sealed class Gun(Firearm itemBase) : Item(itemBase)
{
    public Firearm GameBase { get; } = itemBase;

    public byte Ammo
    {
        get => GameBase.Status.Ammo;
        set => GameBase.Status = new FirearmStatus(value, GameBase.Status.Flags, GameBase.Status.Attachments);
    }

    public bool FlashlightEnabled
    {
        get => GameBase.Status.Flags.HasFlagFast(FirearmStatusFlags.FlashlightEnabled);
        set => GameBase.OverrideFlashlightFlags(value);
    }

    public Attachment[] Attachments
    {
        get => GameBase.Attachments;
        set => GameBase.Attachments = value;
    }

    public float FireRate
    {
        get => Base is AutomaticFirearm automaticFirearm ? automaticFirearm._fireRate : 1;
        set
        {
            if (Base is AutomaticFirearm automaticFirearm)
            {
                automaticFirearm._fireRate = value;
                return;
            }

            // TODO: Заменить лог исключением
            Log.Warn("You cannot change the fire rate of non-automatic weapons.");
        }
    }

    public RecoilSettings Recoil
    {
        get => Base is AutomaticFirearm automaticFirearm ? automaticFirearm._recoil : default;
        set
        {
            if (Base is AutomaticFirearm automaticFirearm)
            {
                automaticFirearm.ActionModule = new AutomaticAction(GameBase, automaticFirearm._semiAutomatic,
                    automaticFirearm._boltTravelTime, 1f / automaticFirearm._fireRate,
                    automaticFirearm._dryfireClipId, automaticFirearm._triggerClipId,
                    automaticFirearm._gunshotPitchRandomization,
                    value, automaticFirearm._recoilPattern, false, Math.Max(1, automaticFirearm._chamberSize));
                return;
            }

            // TODO: Заменить лог исключением
            Log.Warn("You cannot change the recoil pattern of non-automatic weapons.");
        }
    }

    public AmmoType AmmoType => GameBase.AmmoType.GetAmmoType();
    public byte MaxAmmo => GameBase.AmmoManagerModule.MaxAmmo;
}