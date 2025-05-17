using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PlayerRoles;
using Qurre.API.Controllers;

namespace Qurre.API.Addons.Audio.Objects;

/// <summary>
///     Conditions to determine player access.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="AccessConditions" /> class.
/// </remarks>
[PublicAPI]
public class AccessConditions(
    List<int>? ids = null,
    List<uint>? networkIds = null,
    List<string>? userIds = null,
    List<RoleTypeId>? roles = null,
    List<Team>? teams = null,
    List<string>? tags = null,
    List<ItemType>? items = null) : IAccessConditions
{
    public List<int> Ids { get; set; } = ids ?? [];

    public List<uint> NetworkIds { get; set; } = networkIds ?? [];

    public List<string> UserIds { get; set; } = userIds ?? [];

    public List<RoleTypeId> Roles { get; set; } = roles ?? [];

    public List<Team> Teams { get; set; } = teams ?? [];

    public List<string> Tags { get; set; } = tags ?? [];

    public List<ItemType> Items { get; set; } = items ?? [];

    /// <summary>
    ///     Check the player for satisfaction with all conditions.
    /// </summary>
    /// <param name="referenceHub"><see cref="ReferenceHub" /> to check</param>
    /// <returns>Is <see cref="ReferenceHub" /> satisfying?</returns>
    public virtual bool CheckRequirements(ReferenceHub referenceHub)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (referenceHub == null) return false;

        bool allowed = Ids.Contains(referenceHub.PlayerId);

        allowed |= NetworkIds.Contains(referenceHub.netId);

        allowed |= referenceHub.authManager != null &&
                   UserIds.Contains(referenceHub.authManager.UserId);

        allowed |= referenceHub.roleManager?.CurrentRole != null &&
                   Roles.Contains(referenceHub.roleManager.CurrentRole.RoleTypeId);

        allowed |= referenceHub.roleManager?.CurrentRole != null &&
                   Teams.Contains(referenceHub.roleManager.CurrentRole.Team);

        allowed |= Tags.Any() &&
                   Player.List.Any(player =>
                       player.ReferenceHub == referenceHub && Tags.Any(tag => player.Tag.Contains(tag)));

        allowed |= Items.Any() &&
                   (referenceHub.inventory?.UserInventory?.Items?.Any(item =>
                       Items.Any(requiredItem => requiredItem.Equals(item.Value?.ItemTypeId)
                       )) ?? false);

        return allowed;
    }
}