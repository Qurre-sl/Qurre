using JetBrains.Annotations;

namespace Qurre.Events.Structs;

[PublicAPI]
public interface ICancellableEvent : IBaseEvent
{
    /// <summary>
    ///     Gets or sets whether the event is allowed to proceed
    /// </summary>
    bool IsAllowed { get; set; }
}