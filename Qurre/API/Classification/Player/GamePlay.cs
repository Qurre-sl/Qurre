using InventorySystem.Disarming;
using InventorySystem;

namespace Qurre.API.Classification.Player
{
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using MapGeneration;
    using Qurre.API;
    using Qurre.API.Addons.Items;
    using Qurre.API.Controllers;
    using Qurre.API.Objects;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GamePlay
    {
        private readonly Player _player;
        internal GamePlay(Player pl) => _player = pl;

        public Inventory Inventory => _player.ReferenceHub.inventory;

        public Player Cuffer
        {
            get
            {
                foreach (DisarmedPlayers.DisarmedEntry disarmed in DisarmedPlayers.Entries)
                    if (disarmed.DisarmedPlayer == _player.ReferenceHub.netId)
                        return disarmed.Disarmer.GetPlayer();

                return null;
            }
            set
            {
                for (int i = 0; i < DisarmedPlayers.Entries.Count; i++)
                {
                    if (DisarmedPlayers.Entries[i].DisarmedPlayer == Inventory.netId)
                    {
                        DisarmedPlayers.Entries.RemoveAt(i);
                        break;
                    }
                }

                if (value != null)
                    Inventory.SetDisarmedStatus(value.GamePlay.Inventory);
            }
        }
        public bool Cuffed => _player.ReferenceHub.inventory.IsDisarmed();
        public bool Overwatch
        {
            get => _player.ReferenceHub.serverRoles.IsInOverwatch;
            set => _player.ReferenceHub.serverRoles.IsInOverwatch = value;
        }

        public bool GodMode
        {
            get => _player.ClassManager.GodMode;
            set => _player.ClassManager.GodMode = value;
        }

        public ZoneType CurrentZone => Room?.Zone ?? ZoneType.Unknown;

        public Room Room
        {
            get => RoomIdUtils.RoomAtPosition(_player.MovementState.Position).GetRoom() ??
                Map.Rooms.OrderBy(x => Vector3.Distance(x.Position, _player.MovementState.Position)).FirstOrDefault();
            set => _player.MovementState.Position = value.Position + Vector3.up * 2;
        }

        public void ClearInventory()
        {
            while (Inventory.UserInventory.Items.Count > 0)
                Inventory.ServerRemoveItem(Inventory.UserInventory.Items.ElementAt(0).Key, null);
        }

        public Item AddItem(ItemType itemType)
        {
            Item item = Item.Get(Inventory.ServerAddItem(itemType));
            if (item is Gun gun)
            {
                if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(_player.ReferenceHub, out Dictionary<ItemType, uint> _d)
                    && _d.TryGetValue(itemType, out uint _y))
                    gun.Base.ApplyAttachmentsCode(_y, true);
                FirearmStatusFlags status = FirearmStatusFlags.MagazineInserted;
                if (gun.Base.HasAdvantageFlag(AttachmentDescriptiveAdvantages.Flashlight))
                    status |= FirearmStatusFlags.FlashlightEnabled;
                gun.Base.Status = new FirearmStatus(gun.MaxAmmo, status, gun.Base.GetCurrentAttachmentsCode());
            }
            return item;
        }
        public Item AddItem(ItemBase itemBase)
        {
            if (itemBase?.PickupDropModel == null)
                return null;

            Inventory.UserInventory.Items[itemBase.PickupDropModel.NetworkInfo.Serial] = itemBase;

            itemBase.OnAdded(itemBase.PickupDropModel);
            if (itemBase is Firearm)
                AttachmentsServerHandler.SetupProvidedWeapon(_player.ReferenceHub, itemBase);

            Inventory.SendItemsNextFrame = true;
            return Item.Get(itemBase);
        }
        public void AddItem(Item item, int amount)
        {
            if (item == null || amount < 0)
                return;

            for (int i = 0; i < amount; i++)
            {
                AddItem(item.Base);
            }
        }
        public void AddItem(IEnumerable<Item> items)
        {
            if (items == null || items.Count() == 0)
                return;

            foreach (Item item in items)
            {
                AddItem(item.Base);
            }
        }

        public void DropItem(Item item)
            => Inventory.ServerDropItem(item.Serial);
        public bool HasItem(ItemType item)
            => Inventory.UserInventory.Items.Any(tempItem => tempItem.Value.ItemTypeId == item);
    }
}