using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Qurre.API.Addons.Audio.Objects;

/// <summary>
///     Blacklist for <see cref="AudioTask" />.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="AudioTaskBlacklist" /> class.
/// </remarks>
[PublicAPI]
public class AudioTaskBlacklist(List<IAccessConditions>? accessConditions = null)
{
    /// <summary>
    ///     A collection of required conditions to define a player as blacklisted.
    /// </summary>
    public List<IAccessConditions> AccessConditions { get; set; } = accessConditions ?? [];

    /// <summary>
    ///     Check <see cref="ReferenceHub" /> for blacklisting.
    /// </summary>
    /// <param name="referenceHub"><see cref="ReferenceHub" /> to check</param>
    /// <returns>Is <see cref="ReferenceHub" /> blacklisted?</returns>
    public virtual bool Contains(ReferenceHub referenceHub)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (referenceHub == null)
            return true;

        return AccessConditions.Any() && AccessConditions.All(condition => condition.CheckRequirements(referenceHub));
    }
}