using InventorySystem.Items.Pickups;
using InventorySystem.Items;
using InventorySystem;
using System.Collections.Generic;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public sealed class Pickup
    {
        internal static readonly Dictionary<ItemPickupBase, Pickup> BaseToItem = new();

        public ItemPickupBase Base { get; }

        public GameObject GameObject => Base.gameObject;
        public ItemCategory Category => _bufferedCategory == ItemCategory.None ?
                                        _bufferedCategory = Type.GetCategory() :
                                        _bufferedCategory;

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
            get => Base.NetworkInfo.ItemId;
            set
            {
                PickupSyncInfo syncInfo = Base.Info;

                syncInfo.ItemId = value;
                Base.NetworkInfo = syncInfo;
                _bufferedCategory = value.GetCategory();
            }
        }


        public ushort Serial
        {
            get => _bufferedSerial == 0 ?
                   _bufferedSerial = 0 :
                   _bufferedSerial;
            set
            {
                if (Base == null)
                    return;

                if (value == 0)
                    value = ItemSerialGenerator.GenerateNext();

                PickupSyncInfo syncInfo = Base.Info;

                syncInfo.Serial = value;
                Base.NetworkInfo = syncInfo;
                _bufferedSerial = value;
            }
        }

        public float Weight
        {
            get => Base.NetworkInfo.WeightKg;
            set
            {
                PickupSyncInfo syncInfo = Base.Info;
                syncInfo.WeightKg = value;
                Base.NetworkInfo = syncInfo;
            }
        }

        public bool Locked
        {
            get => Base.NetworkInfo.Locked;
            set
            {
                PickupSyncInfo info = Base.Info;

                info.Locked = value;
                Base.NetworkInfo = info;
            }
        }

        public bool InUse
        {
            get => Base.NetworkInfo.InUse;
            set
            {
                PickupSyncInfo info = Base.Info;

                info.InUse = value;
                Base.NetworkInfo = info;
            }
        }

        public Vector3 Position
        {
            get => Base.Position;
            set => Base.Position = value;
        }

        public Quaternion Rotation
        {
            get => Base.Rotation;
            set => Base.Rotation = value;
        }

        public Vector3 Scale
        {
            get => Base.gameObject.transform.localScale;
            set
            {
                GameObject.transform.localScale = value;

                GameObject.NetworkRespawn();
            }
        }

        private string _tag;
        private ushort _bufferedSerial;
        private ItemCategory _bufferedCategory;

        public Pickup(ItemPickupBase pickupBase)
        {
            Base = pickupBase;
            Serial = pickupBase.NetworkInfo.Serial;
            _bufferedCategory = Type.GetCategory();

            BaseToItem.Add(pickupBase, this);
        }

        public Pickup(ItemType type)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase itemBase))
                return;

            Base = itemBase.PickupDropModel;
            Serial = itemBase.PickupDropModel.NetworkInfo.Serial;

            BaseToItem.Add(itemBase.PickupDropModel, this);
        }

        public static Pickup Get(ItemPickupBase pickupBase) =>
            pickupBase == null ? null :
            BaseToItem.ContainsKey(pickupBase) ? BaseToItem[pickupBase] :
            new Pickup(pickupBase);

        public void Destroy()
        {
            Base.DestroySelf();
            BaseToItem.Remove(Base);
        }


        static internal Pickup SafeGet(ItemPickupBase @base)
        {
            try { return Get(@base); }
            catch (System.Exception e)
            {
                Log.Debug(e);
                return null;
            }
        }
    }
}