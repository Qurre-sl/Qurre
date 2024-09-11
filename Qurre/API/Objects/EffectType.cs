using System;
using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum EffectType : byte
{
    None,
    AmnesiaItems,
    AmnesiaVision,
    AntiScp207,
    Asphyxiated,
    [Obsolete("removed")] BecomingFlamingo,
    Bleeding,
    Blinded,
    BodyshotReduction,
    Burned,
    CardiacArrest,
    Concussed,
    Corroding,
    DamageReduction,
    Deafened,
    Decontaminating,
    Disabled,
    Ensnared,
    Exhausted,
    Flashed,
    FogControl,
    Ghostly,
    Hemorrhage,
    Hypothermia,
    InsufficientLighting,
    Invigorated,
    Invisible,
    MovementBoost,
    PocketCorroding,
    Poisoned,
    RainbowTaste,
    Scanned,
    Scp1853,
    Scp207,
    SeveredHands,
    SilentWalk,
    Sinkhole,
    Slowness,
    SoundtrackMute,
    SpawnProtected,
    Stained,
    Strangled,
    [Obsolete("removed")] Snowed,
    Traumatized,
    Vitality
}