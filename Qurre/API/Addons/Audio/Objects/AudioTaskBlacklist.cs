using System.Collections.Generic;
using System.Linq;

namespace Qurre.API.Addons.Audio.Objects
{
    /// <summary>
    /// Blaclist for <see cref="AudioTask"/>.
    /// </summary>
    public class AudioTaskBlacklist
    {
        /// <summary>
        /// A collection of required conditions to define a player as blacklisted.
        /// </summary>
        public List<IAccessConditions> AccessConditions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioTaskBlacklist"/> class.
        /// </summary>
        public AudioTaskBlacklist(List<IAccessConditions> accessConditions = null)
        {
            AccessConditions = accessConditions ?? new List<IAccessConditions>();
        }

        /// <summary>
        /// Check <see cref="ReferenceHub"/> for blacklisting.
        /// </summary>
        /// <param name="referenceHub"><see cref="ReferenceHub"/> to check</param>
        /// <returns>Is <see cref="ReferenceHub"/> blacklisted?</returns>
        public virtual bool Contains(ReferenceHub referenceHub)
        {
            if (referenceHub == null)
            {
                return true;
            }
            else if (!AccessConditions?.Any() ?? true)
            {
                return false;
            }

            return AccessConditions.All(condition => condition.CheckRequirements(referenceHub));
        }
    }
}