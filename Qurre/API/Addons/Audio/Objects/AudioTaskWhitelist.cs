using System.Collections.Generic;
using System.Linq;

namespace Qurre.API.Addons.Audio.Objects
{
    /// <summary>
    /// Whitelist for <see cref="AudioTask"/>.
    /// </summary>
    public class AudioTaskWhitelist
    {
        /// <summary>
        /// A collection of required conditions to define a player as whitelisted.
        /// </summary>
        public List<AccessConditions> AccessConditions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioTaskWhitelist"/> class.
        /// </summary>
        public AudioTaskWhitelist(List<AccessConditions> accessConditions = null)
        {
            AccessConditions = accessConditions ?? new List<AccessConditions>();
        }

        /// <summary>
        /// Check <see cref="ReferenceHub"/> for whitelisting.
        /// </summary>
        /// <param name="referenceHub"><see cref="ReferenceHub"/> to check</param>
        /// <returns>Is <see cref="ReferenceHub"/> whitelisted?</returns>
        public virtual bool Contains(ReferenceHub referenceHub)
        {
            if (referenceHub == null)
            {
                return false;
            }
            else if (!AccessConditions?.Any() ?? true)
            {
                return true;
            }

            return AccessConditions.All(condition => condition.CheckRequirements(referenceHub));
        }
    }
}