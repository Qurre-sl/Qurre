using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum LockerType : byte
{
    Unknown,
    AdrenalineMedkit,
    RegularMedkit,
    Pedestal,
    MiscLocker,
    RifleRack,
    LargeGun
}