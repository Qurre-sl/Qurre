using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Qurre.API.Addons.Audio.Objects;

/// <summary>
///     Whitelist for <see cref="AudioTask" />.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="AudioTaskWhitelist" /> class.
/// </remarks>
[PublicAPI]
public class AudioTaskWhitelist(List<IAccessConditions>? accessConditions = null)
{
    /// <summary>
    ///     A collection of required conditions to define a player as whitelisted.
    /// </summary>
    public List<IAccessConditions> AccessConditions { get; set; } = accessConditions ?? [];

    /// <summary>
    ///     Check <see cref="ReferenceHub" /> for whitelisting.
    /// </summary>
    /// <param name="referenceHub"><see cref="ReferenceHub" /> to check</param>
    /// <returns>Is <see cref="ReferenceHub" /> whitelisted?</returns>
    public virtual bool Contains(ReferenceHub referenceHub)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (referenceHub == null)
            return false;

        return !AccessConditions.Any() || AccessConditions.All(condition => condition.CheckRequirements(referenceHub));
    }
}