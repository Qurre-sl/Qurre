using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using Qurre.API.Core;
using Qurre.Internal.Attributes;

namespace Qurre.API.Entities.Items.Implementations;

[EntityWrapBindForFactory(typeof(KeycardItem))]
internal sealed class Keycard(KeycardItem itemBase) : Item(itemBase), IKeycard
{
    /// <inheritdoc />
    public new UnityObjectWrapper<KeycardItem> Base { get; } = itemBase;

    /*/// <inheritdoc />
    public KeycardPermissions Permissions
    {
        get => Base.Instance.Permissions;
        set => Base.Instance.Permissions = value;
    }*/ // TODO: переработать
}