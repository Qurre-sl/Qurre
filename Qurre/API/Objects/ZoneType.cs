using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum ZoneType : byte
{
    Surface,
    Office,
    Heavy,
    Light,
    Unknown
}