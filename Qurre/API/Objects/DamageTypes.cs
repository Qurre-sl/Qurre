// ReSharper disable InconsistentNaming

using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum DamageTypes : byte
{
    Unknown,

    Com15,
    Com18,
    Com45,
    Revolver,

    FSP9,
    CrossVec,

    E11SR,
    FRMG0,

    AK,
    Logicer,
    Shotgun,
    A7,

    Custom,
    Disruptor,
    Explosion,
    Jailbird,
    MicroHid,

    Recontainment,
    Scp018,
    Scp049,
    Scp096,
    Warhead,

    Asphyxiation,
    Bleeding,
    Falldown,
    Pocket,
    Decontamination,
    Poison,
    Scp207,
    SeveredHands,
    Tesla,
    Scp173,
    Scp939,
    Scp939Lunge,
    Zombie,
    Crushed,
    FriendlyFireDetector,
    Hypothermia,
    CardiacArrest,
    Scp3114,
    MarshmallowMan,
    Scp1507,
    Snowball
}