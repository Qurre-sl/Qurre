using System;
using System.Collections.Generic;
using System.Linq;
using CameraShaking;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Attachments.Components;
using InventorySystem.Items.Firearms.Modules;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using Qurre.API.Objects;

namespace Qurre.API.Addons.Items;

[PublicAPI]
public sealed class Gun : Item
{
    public new Firearm Base { get; }

    #region Cached Modules

    public ModuleBase[] Modules
    {
        get => Base.Modules;
        set => Base._modules = value;
    }

    public IActionModule? ActionModule { get; private set; }
    public IAdsModule? AdsModule { get; private set; }
    public IAdsPreventerModule[] AdsPreventerModule { get; private set; } = [];
    public IAmmoContainerModule[] AmmoContainerModule { get; private set; } = [];
    public IBusyIndicatorModule[] BusyIndicatorModule { get; private set; } = [];
    public IDisplayableAmmoProviderModule[] DisplayableAmmoProviderModule { get; private set; } = [];
    public IDisplayableInaccuracyProviderModule[] DisplayableInaccuracyProviderModule { get; private set; } = [];
    public IDisplayableRecoilProviderModule? DisplayableRecoilProviderModule { get; private set; }
    public IEquipperModule? EquipperModule { get; private set; }
    public IHitregModule? HitregModule { get; private set; }
    public IInaccuracyProviderModule[] InaccuracyProviderModule { get; private set; } = [];
    public IInspectPreventerModule[] InspectPreventerModule { get; private set; } = [];
    public IMagazineControllerModule? MagazineControllerModule { get; private set; }
    public IPrimaryAmmoContainerModule? PrimaryAmmoContainerModule { get; private set; }
    public IRecoilScalingModule[] RecoilScalingModule { get; private set; } = [];
    public IRecommendableModule[] RecommendableModule { get; private set; } = [];
    public IReloaderModule? ReloaderModule { get; private set; }
    public IReloadUnloadValidatorModule[] ReloadUnloadValidatorModule { get; private set; } = [];
    public ISpectatorSyncModule? SpectatorSyncModule { get; private set; }
    public ISwayModifierModule[] SwayModifierModule { get; private set; } = [];
    public ITriggerControllerModule? TriggerControllerModule { get; private set; }
    public ITriggerPressPreventerModule[] TriggerPressPreventerModule { get; private set; } = [];

    // IActionModule
    public AutomaticActionModule? AutomaticActionModule => ActionModule as AutomaticActionModule;
    public DisruptorActionModule? DisruptorActionModule => ActionModule as DisruptorActionModule;
    public DoubleActionModule? DoubleActionModule => ActionModule as DoubleActionModule;
    public PumpActionModule? PumpActionModule => ActionModule as PumpActionModule;
    
    // IAdsModule
    public LinearAdsModule? LinearAdsModule => AdsModule as LinearAdsModule;
    
    // IDisplayableRecoilProviderModule
    public RecoilPatternModule? RecoilPatternModule => DisplayableRecoilProviderModule as RecoilPatternModule;
    
    // IMagazineControllerModule
    public MagazineModule? MagazineModule => MagazineControllerModule as MagazineModule;
    
    public void RefreshCachedModules()
    {
        ActionModule = GetModuleNullable<IActionModule>();
        AdsPreventerModule = GetModules<IAdsPreventerModule>();
        AdsModule = GetModuleNullable<IAdsModule>();
        AmmoContainerModule = GetModules<IAmmoContainerModule>();
        BusyIndicatorModule = GetModules<IBusyIndicatorModule>();
        DisplayableAmmoProviderModule = GetModules<IDisplayableAmmoProviderModule>();
        DisplayableInaccuracyProviderModule = GetModules<IDisplayableInaccuracyProviderModule>();
        DisplayableRecoilProviderModule = GetModuleNullable<IDisplayableRecoilProviderModule>();
        EquipperModule = GetModuleNullable<IEquipperModule>();
        HitregModule = GetModuleNullable<IHitregModule>();
        InaccuracyProviderModule = GetModules<IInaccuracyProviderModule>();
        InspectPreventerModule = GetModules<IInspectPreventerModule>();
        MagazineControllerModule = GetModuleNullable<IMagazineControllerModule>();
        PrimaryAmmoContainerModule = GetModuleNullable<IPrimaryAmmoContainerModule>();
        RecommendableModule = GetModules<IRecommendableModule>();
        ReloaderModule = GetModuleNullable<IReloaderModule>();
        ReloadUnloadValidatorModule = GetModules<IReloadUnloadValidatorModule>();
        SpectatorSyncModule = GetModuleNullable<ISpectatorSyncModule>();
        SwayModifierModule = GetModules<ISwayModifierModule>();
        TriggerControllerModule = GetModuleNullable<ITriggerControllerModule>();
        TriggerPressPreventerModule = GetModules<ITriggerPressPreventerModule>();
    }

    public TModule? GetModuleNullable<TModule>() where TModule : class
    {
        return Base.TryGetModule<TModule>(out var module) ? module : null;
    }

    public TModule[] GetModules<TModule>() where TModule : class
    {
        return Base.Modules.OfType<TModule>().ToArray();
    }
    
    #endregion

    #region Cached Attachments

    public Attachment[] Attachments
    {
        get => Base.Attachments;
        set => Base._attachments = value;
    }

    public FlashlightAttachment? FlashlightAttachment { get; private set; }
    public IReadOnlyDictionary<AttachmentSlot, Attachment[]> AttachmentsBySlot { get; private set; }
        = new Dictionary<AttachmentSlot, Attachment[]>();

    public Attachment? GetActiveAttachment(AttachmentSlot slot)
    {
        return AttachmentsBySlot.TryGetValue(slot, out var slotAttachments) 
            ? slotAttachments.FirstOrDefault(attachment => attachment.isActiveAndEnabled) 
            : null;
    }

    public void RefreshCachedAttachments()
    {
        FlashlightAttachment = Base.Attachments.OfType<FlashlightAttachment>().FirstOrDefault();
        
        AttachmentsBySlot = Base.Attachments
            .GroupBy(attachment => attachment.Slot)
            .ToDictionary(group => group.Key, group => group.ToArray());
    }

    #endregion
    
    public int TotalStoredAmmo => AmmoContainerModule.Sum(container => container.AmmoStored);
    public int TotalMaxAmmo => AmmoContainerModule.Sum(container => container.AmmoMax);
    
    public int Ammo
    {
        get => PrimaryAmmoContainerModule?.AmmoStored ?? throw new NullReferenceException("PrimaryAmmoContainerModule is null");
        set
        {
            if (PrimaryAmmoContainerModule == null)
                throw new NullReferenceException("PrimaryAmmoContainerModule is null");
            PrimaryAmmoContainerModule.ServerModifyAmmo(value);
        }
    }

    public bool FlashlightEnabled
    {
        get => FlashlightAttachment?.IsEmittingLight ?? throw new NullReferenceException("FlashlightAttachment is null");
        set
        {
            if (FlashlightAttachment == null)
                throw new NullReferenceException("FlashlightAttachment is null");
            FlashlightAttachment.IsEnabled = value;
            FlashlightAttachment.ServerSendStatus(true);
        }
    }

    public bool MagazineInserted
    {
        get => MagazineControllerModule?.MagazineInserted ?? throw new NullReferenceException("MagazineControllerModule is null");
        set
        {
            if (MagazineModule == null)
                throw new NullReferenceException("MagazineModule is null");
            MagazineModule.MagazineInserted = value;
            MagazineModule.ServerResyncData();
        }
    }

    public float FireRate
    {
        get => AutomaticActionModule?.BaseFireRate ?? throw new NullReferenceException("AutomaticActionModule is null");
        set
        {
            if (AutomaticActionModule == null)
                throw new NullReferenceException("You cannot change the fire rate of non-automatic weapons.");
            AutomaticActionModule.BaseFireRate = value;
        }
    }

    public RecoilSettings Recoil
    {
        get => RecoilPatternModule?.BaseRecoil ?? throw new NullReferenceException("RecoilPatternModule is null");
        set
        {
            if (RecoilPatternModule == null)
                throw new NullReferenceException("RecoilPatternModule is null");
            RecoilPatternModule.BaseRecoil = value;
        }
    }

    public AmmoType AmmoType => PrimaryAmmoContainerModule?.AmmoType.GetAmmoType() ?? AmmoType.None;
    
    public int MaxAmmo => PrimaryAmmoContainerModule?.AmmoMax ?? throw new NullReferenceException("PrimaryAmmoContainerModule is null");

    public Gun(Firearm itemBase) : base(itemBase)
    {
        Base = itemBase;
        RefreshCachedModules();
        RefreshCachedAttachments();
    }
}