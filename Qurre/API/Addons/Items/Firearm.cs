using CameraShaking;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Attachments.Components;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using System;
using FirearmBase = InventorySystem.Items.Firearms.Firearm;

namespace Qurre.API.Addons.tems
{
    public sealed class Firearm : Item
    {
        public new FirearmBase Base { get; }

        public byte Ammo
        {
            get => Base.Status.Ammo;
            set => Base.Status = new FirearmStatus(value, Base.Status.Flags, Base.Status.Attachments);
        }

        public bool FlashlightEnabled
        {
            get => Base.Status.Flags.HasFlagFast(FirearmStatusFlags.FlashlightEnabled);
            set => Base.OverrideFlashlightFlags(true);
        }

        public Attachment[] Attachments
        {
            get => Base.Attachments;
            set => Base.Attachments = value;
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
                Log.Warn("You cannot change the firerate of non-automatic weapons.");
            }
        }

        public RecoilSettings Recoil
        {
            get => Base is AutomaticFirearm automaticFirearm ? automaticFirearm._recoil : default;
            set
            {
                if (Base is AutomaticFirearm automaticFirearm)
                {
                    automaticFirearm.ActionModule = new AutomaticAction(Base, automaticFirearm._semiAutomatic, automaticFirearm._boltTravelTime, 1f / automaticFirearm._fireRate,
                                                                        automaticFirearm._dryfireClipId, automaticFirearm._triggerClipId, automaticFirearm._gunshotPitchRandomization,
                                                                        value, automaticFirearm._recoilPattern, false, Math.Max(1, automaticFirearm._chamberSize));
                    return;
                }

                // TODO: Заменить лог исключением
                Log.Warn("You cannot change the recoil pattern of non-automatic weapons.");
            }
        }

        public AmmoType AmmoType => Base.AmmoType.GetAmmoType();
        public byte MaxAmmo => Base.AmmoManagerModule.MaxAmmo;

        public Firearm(FirearmBase itemBase) : base(itemBase)
        {
            Base = itemBase;
        }
    }
}
