using JetBrains.Annotations;

namespace Qurre.API.Objects;

[PublicAPI]
public enum ScpAttackType : byte
{
    Scp049,
    Scp0492,
    Scp096,
    Scp173,
    Scp939
}