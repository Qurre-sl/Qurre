using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using Qurre.API.Controllers;

namespace Qurre.API.Addons.Items
{
    public sealed class Keycard : Item
    {
        public Keycard(KeycardItem itemBase) : base(itemBase)
            => Base = itemBase;

        public Keycard(ItemType itemType) : this((KeycardItem)itemType.CreateItemInstance()) { }

        public new KeycardItem Base { get; }

        public KeycardPermissions Permissions
        {
            get => Base.Permissions;
            set => Base.Permissions = value;
        }
    }
}