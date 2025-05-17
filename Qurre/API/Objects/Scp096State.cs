using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum Scp096State
{
    Unknown = 0,

    Charging = 1,
    Enraging = 2,

    TryNotCry = 3,
    StartCrying = 4,

    Calming = 5,
    Enraged = 6,
    Distressed = 7,
    Docile = 8
}