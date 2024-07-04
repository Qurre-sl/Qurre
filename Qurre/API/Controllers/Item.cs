using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.MicroHID;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Radio;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.ToggleableLights.Flashlight;
using InventorySystem.Items.Usables;
using Mirror;
using Qurre.API.Addons.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Item
    {
        internal static readonly Dictionary<ItemBase, Item> BaseToItem = new();

        public ItemBase Base { get; }

        public ItemCategory Category => Base.Category;
        public float Weight => Base.Weight;

        public string Tag
        {
            get => _tag;
            set
            {
                if (value is null)
                    return;

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
            get
            {
                return Base.ItemSerial;
            }
            internal set
            {
                if (value == 0) value = ItemSerialGenerator.GenerateNext();
                if (Base == null || Base.PickupDropModel == null)
                    return;
                Base.PickupDropModel.Info.Serial = value;
                try { Base.PickupDropModel.NetworkInfo = Base.PickupDropModel.Info; } catch { }
            }
        }

        public Player Owner
        {
            get
            {
                if (Base?.Owner != null)
                    return Base.Owner.GetPlayer();

                return Server.Host;
            }
        }

        public Pickup Pickup
        {
            get
            {
                if (Pickup.BaseToItem.TryGetValue(Base.PickupDropModel, out var pick))
                    return pick;

                return null;
            }
        }

        private string _tag;

        public Item(ItemBase itemBase)
        {
            Base = itemBase;
            Serial = Base.OwnerInventory.UserInventory.Items.FirstOrDefault(i => i.Value == Base).Key;
            if (Serial == 0)
                Serial = ItemSerialGenerator.GenerateNext();

            BaseToItem.Add(itemBase, this);
        }

        public Item(ItemType type)
            : this(Server.InventoryHost.CreateItemInstance(new(type, ItemSerialGenerator.GenerateNext()), true)) { }

        static internal Item SafeGet(ItemBase itemBase)
        {
            try { return Get(itemBase); }
            catch (System.Exception e)
            {
                Log.Debug(e);
                return null;
            }
        }

        public static Item Get(ItemBase itemBase)
        {
            if (itemBase == null)
                return null;

            if (BaseToItem.ContainsKey(itemBase))
                return BaseToItem[itemBase];

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
                        _ => new Throwable(throwable),
                    };
                default:
                    return new Item(itemBase);
            }
        }

        static public Item Get(ushort serial)
        {
            if (Object.FindObjectsOfType<ItemBase>().TryFind(out var item, x => x.ItemSerial == serial))
                return SafeGet(item);

            return null;
        }

        public void Give(Player player) => player.Inventory.AddItem(Base);

        public virtual Pickup Spawn(Vector3 position, Quaternion rotation = default, Vector3 scale = default)
        {
            ItemPickupBase ipb = Object.Instantiate(Base.PickupDropModel, position, rotation);

            ipb.Info.ItemId = Type;
            ipb.Info.WeightKg = Weight;
            ipb.NetworkInfo = ipb.Info;

            ipb.Position = position;
            ipb.Rotation = rotation;

            if (ipb is FirearmPickup firearmPickup)
            {
                firearmPickup.NetworkStatus = firearmPickup.Status;
            }

            NetworkServer.Spawn(ipb.gameObject);

            Pickup pickup = Pickup.Get(ipb);
            pickup.Scale = scale == default ? Vector3.one : scale;
            return pickup;
        }

        public override bool Equals(object obj)
        {
            return obj is Item item && Serial == item?.Serial;
        }

        public override int GetHashCode()
        {
            return -29014143 + Serial.GetHashCode();
        }

        public static bool operator ==(Item a, Item b)
        {
            return a?.Serial == b?.Serial;
        }

        public static bool operator !=(Item a, Item b)
        {
            return !(a == b);
        }
    }
}