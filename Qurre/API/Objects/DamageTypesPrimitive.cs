using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum LiteDamageTypes : byte
{
    Unknown,
    Custom,
    Disruptor,
    Explosion,
    Gun,
    MicroHid,
    Recontainment,
    Scp018,
    Scp049,
    Scp096,
    ScpDamage,
    Universal,
    Warhead,
    Jailbird,
    Snowball
}