using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum RadioStatus : sbyte
{
    Disabled = -1,
    Short,
    Medium,
    Long,
    Ultra
}