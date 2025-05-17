using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using JetBrains.Annotations;
using Qurre.API.Core;

namespace Qurre.API.Entities.Items;

[PublicAPI]
public interface IKeycard : IItem
{
    new UnityObjectWrapper<KeycardItem> Base { get; }

    /*KeycardPermissions Permissions { get; set; }*/ // TODO: переработать
}