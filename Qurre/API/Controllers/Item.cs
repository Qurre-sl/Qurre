using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Flashlight;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.MicroHID;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Radio;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables;
using Mirror;
using Qurre.API.Addons.Items;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers
{
    public class Item
    {
        internal static readonly Dictionary<ItemBase, Item> BaseToItem = new ();

        private string _tag;

        public Item(ItemBase itemBase)
        {
            Base = itemBase;
            Serial = Base.OwnerInventory.UserInventory.Items.FirstOrDefault(i => i.Value == Base).Key;

            if (Serial == 0)
            {
                Serial = ItemSerialGenerator.GenerateNext();
            }

            BaseToItem.Add(itemBase, this);
        }

        public Item(ItemType type)
            : this(Server.InventoryHost.CreateItemInstance(new (type, ItemSerialGenerator.GenerateNext()), false)) { }

        public ItemBase Base { get; }

        public ItemCategory Category => Base.Category;
        public float Weight => Base.Weight;

        public string Tag
        {
            get => _tag;
            set
            {
                if (value is null)
                {
                    return;
                }

                _tag = value;
            }
        }

        public ItemType Type
        {
            get => Base.ItemTypeId;
            set
            {
                // TODO
            }
        }

        public ushort Serial
        {
            get => Base.OwnerInventory.UserInventory.Items.FirstOrDefault(i => i.Value == Base).Key;
            internal set
            {
                if (value == 0)
                {
                    value = ItemSerialGenerator.GenerateNext();
                }

                if (Base == null || Base.PickupDropModel == null)
                {
                    return;
                }

                Base.PickupDropModel.Info.Serial = value;

                try
                {
                    Base.PickupDropModel.NetworkInfo = Base.PickupDropModel.Info;
                }
                catch { }
            }
        }

        public Player Owner
        {
            get
            {
                if (Base?.Owner != null)
                {
                    return Base.Owner.GetPlayer();
                }

                return Server.Host;
            }
        }

        public Pickup Pickup =>
            // TODO
            null;

        public static Item Get(ItemBase itemBase)
        {
            if (itemBase == null)
            {
                return null;
            }

            if (BaseToItem.ContainsKey(itemBase))
            {
                return BaseToItem[itemBase];
            }

            switch (itemBase)
            {
                case Firearm gun:
                    return new Gun(gun);
                case KeycardItem card:
                    return new Keycard(card);
                case UsableItem usable:
                {
                    //if (usable is Scp330Bag bag)
                    //    return new Scp330(bag);
                    return new Usable(usable);
                }
                case RadioItem radio:
                    return new Radio(radio);
                case MicroHIDItem hid:
                    return new MicroHID(hid);
                case BodyArmor armor:
                    return new Armor(armor);
                case AmmoItem ammo:
                    return new Ammo(ammo);
                case FlashlightItem flashlight:
                    return new Flashlight(flashlight);
                case ThrowableItem throwable:
                    return throwable.Projectile switch
                    {
                        FlashbangGrenade _ => new GrenadeFlash(throwable),
                        ExplosionGrenade _ => new GrenadeFrag(throwable),
                        _ => new Throwable(throwable)
                    };
                default:
                    return new (itemBase);
            }
        }

        public static Item Get(ushort serial)
        {
            if (Object.FindObjectsOfType<ItemBase>().TryFind(out ItemBase item, x => x.ItemSerial == serial))
            {
                return SafeGet(item);
            }

            return null;
        }

        public static bool operator ==(Item a, Item b)
            => a?.Serial == b?.Serial;

        public static bool operator !=(Item a, Item b)
            => !(a == b);

        public void Give(Player player) => player.Inventory.AddItem(Base);

        public virtual Pickup Spawn(Vector3 position, Quaternion rotation = default, Vector3 scale = default)
        {
            PickupSyncInfo syncInfo = Base.PickupDropModel.Info;
            syncInfo.ItemId = Type;
            syncInfo.Weight = Weight;
            syncInfo._serverPosition = position;
            syncInfo._serverRotation = rotation;
            Base.PickupDropModel.NetworkInfo = syncInfo;

            ItemPickupBase ipb = Object.Instantiate(Base.PickupDropModel, position, rotation);

            if (ipb is FirearmPickup firearmPickup)
            {
                if (this is Gun firearm)
                {
                    firearmPickup.Status = new (firearm.Ammo, FirearmStatusFlags.MagazineInserted, firearmPickup.Status.Attachments);
                }
                else
                {
                    byte ammo = Base switch
                    {
                        AutomaticFirearm auto => auto._baseMaxAmmo,
                        Shotgun shotgun => shotgun._ammoCapacity,
                        Revolver _ => 6,
                        _ => 0
                    };
                    firearmPickup.Status = new (ammo, FirearmStatusFlags.MagazineInserted, firearmPickup.Status.Attachments);
                }

                firearmPickup.NetworkStatus = firearmPickup.Status;
            }

            NetworkServer.Spawn(ipb.gameObject);
            ipb.InfoReceived(default, Base.PickupDropModel.NetworkInfo);
            var pickup = Pickup.Get(ipb);
            pickup.Scale = scale == default ? Vector3.one : scale;
            return pickup;
        }

        public override bool Equals(object obj)
            => obj is Item item && Serial == item?.Serial;

        public override int GetHashCode()
            => -29014143 + Serial.GetHashCode();

        internal static Item SafeGet(ItemBase itemBase)
        {
            try
            {
                return Get(itemBase);
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return null;
            }
        }
    }
}