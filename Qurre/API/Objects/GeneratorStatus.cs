using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum GeneratorStatus : byte
{
    Activate,
    Deactivate,
    Unlock,
    OpenDoor,
    CloseDoor
}