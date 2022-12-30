using InventorySystem.Disarming;
using InventorySystem;

namespace Qurre.API.Classification.Player
{
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms.Attachments;
    using Qurre.API;
    using Qurre.API.Controllers;
    using System.Collections.Generic;
    using System.Linq;

    public class GamePlay
    {
        private readonly Player _player;
        internal GamePlay(Player pl) => _player = pl;

        public Inventory Inventory => _player.ReferenceHub.inventory;

        public bool Cuffed => _player.ReferenceHub.inventory.IsDisarmed();
        public bool Overwatch
        {
            get => _player.ReferenceHub.serverRoles.OverwatchEnabled;
            set => _player.ReferenceHub.serverRoles.SetOverwatchStatus(value);
        }

        public Item AddItem(ItemBase itemBase)
        {
            Item item = Item.Get(itemBase);
            Inventory.UserInventory.Items[itemBase.PickupDropModel.NetworkInfo.Serial] = itemBase;

            itemBase.OnAdded(itemBase.PickupDropModel);
            if (itemBase is InventorySystem.Items.Firearms.Firearm)
                AttachmentsServerHandler.SetupProvidedWeapon(_player.ReferenceHub, itemBase);

            Inventory.SendItemsNextFrame = true;
            return item;
        }
        public void AddItem(Item item, int amount)
        {
            if (0 >= amount) return;
            for (int i = 0; i < amount; i++)
                AddItem(item.Base);
        }
        public void AddItem(List<Item> items)
        {
            if (0 >= items.Count) return;
            for (int i = 0; i < items.Count; i++)
                AddItem(items[i].Base);
        }
        public void DropItem(Item item) => Inventory.ServerDropItem(item.Serial);
        public bool HasItem(ItemType item) => Inventory.UserInventory.Items.Any(tempItem => tempItem.Value.ItemTypeId == item);
    }
}