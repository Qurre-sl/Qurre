using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using JetBrains.Annotations;
using Qurre.API.Controllers;

namespace Qurre.API.Addons.Items;

[PublicAPI]
public sealed class Keycard(KeycardItem itemBase) : Item(itemBase)
{
    public Keycard(ItemType itemType) : this((KeycardItem)itemType.CreateItemInstance())
    {
    }

    public KeycardItem GameBase { get; } = itemBase;

    public KeycardPermissions Permissions
    {
        get => GameBase.Permissions;
        set => GameBase.Permissions = value;
    }
}