using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum WorkstationStatus : byte
{
    Offline,
    PoweringUp,
    PoweringDown,
    Online
}