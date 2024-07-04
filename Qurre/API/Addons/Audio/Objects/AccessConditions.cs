using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace Qurre.API.Addons.Audio.Objects
{
    /// <summary>
    /// Conditions to determine player access.
    /// </summary>
    public class AccessConditions : IAccessConditions
    {
        public List<int> Ids { get; set; }

        public List<uint> NetworkIds { get; set; }

        public List<string> UserIds { get; set; }

        public List<RoleTypeId> Roles { get; set; }

        public List<Team> Teams { get; set; }

        public List<string> Tags { get; set; }

        public List<ItemType> Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessConditions"/> class.
        /// </summary>
        public AccessConditions(List<int> ids = null, List<uint> networkIds = null,
            List<string> userIds = null, List<RoleTypeId> roles = null,
            List<Team> teams = null, List<string> tags = null, List<ItemType> items = null)
        {
            Ids = ids;
            NetworkIds = networkIds;
            UserIds = userIds;
            Roles = roles;
            Teams = teams;
            Tags = tags;
            Items = items;
        }

        /// <summary>
        /// Check the player for satisfaction with all conditions.
        /// </summary>
        /// <param name="referenceHub"><see cref="ReferenceHub"/> to check</param>
        /// <returns>Is <see cref="ReferenceHub"/> satisfying?</returns>
        public virtual bool CheckRequirements(ReferenceHub referenceHub)
        {
            if (referenceHub == null)
            {
                return false;
            }

            var allowed = Ids?.Contains(referenceHub.PlayerId) ?? false;

            allowed |= NetworkIds?.Contains(referenceHub.netId) ?? false;

            allowed |= (referenceHub.authManager != null) &&
                (UserIds?.Contains(referenceHub.authManager.UserId) ?? false);

            allowed |= (referenceHub.roleManager?.CurrentRole != null) &&
                (Roles?.Contains(referenceHub.roleManager.CurrentRole.RoleTypeId) ?? false);

            allowed |= (referenceHub.roleManager?.CurrentRole != null) &&
                (Teams?.Contains(referenceHub.roleManager.CurrentRole.Team) ?? false);

            allowed |= (Tags?.Any() ?? false) &&
                Player.List.Any(player => player.ReferenceHub == referenceHub && Tags.Any(tag => player.Tag.Contains(tag)));

            allowed |= (Items?.Any() ?? false) &&
                (referenceHub.inventory?.UserInventory?.Items?.Any(
                    item => Items.Any(requiredItem => requiredItem.Equals(item.Value?.ItemTypeId)
                    )) ?? false);

            return allowed;
        }
    }
}