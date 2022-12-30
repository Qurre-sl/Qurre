using InventorySystem;
using Qurre.API.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;

// AlexanderK: maybe I'll do it later 
namespace Qurre.API.Classification.Player
{
    public sealed class InventoryItems : IDictionary<byte, Item>, ICollection<KeyValuePair<byte, Item>>, IEnumerable<KeyValuePair<byte, Item>>, IEnumerable, IReadOnlyDictionary<byte, Item>, IReadOnlyCollection<KeyValuePair<byte, Item>>
    {
        public int BusySlotsCount { get; }
        public int FreeSlotsCount => MaxSlotsCount - BusySlotsCount;

        private int MaxSlotsCount => Inventory.MaxSlots;
        public ICollection<Item> Values => _items.Values;

        int ICollection<KeyValuePair<byte, Item>>.Count => MaxSlotsCount;
        int IReadOnlyCollection<KeyValuePair<byte, Item>>.Count => MaxSlotsCount;
        bool ICollection<KeyValuePair<byte, Item>>.IsReadOnly => true;

        ICollection<byte> IDictionary<byte, Item>.Keys => _items.Keys;
        IEnumerable<byte> IReadOnlyDictionary<byte, Item>.Keys => _items.Keys;
        IEnumerable<Item> IReadOnlyDictionary<byte, Item>.Values => _items.Values;


        private readonly IDictionary<byte, Item> _items;
        private readonly Inventory _inventory;

        internal InventoryItems(Inventory inventory)
        {
            _inventory = inventory;
            _items = new Dictionary<byte, Item>(MaxSlotsCount);
        }

        public Item this[byte slotId]
        {
            get
            {
                return _items[slotId];
            }
            set
            {
                if (slotId < 0 || slotId > MaxSlotsCount)
                    return;

                // TODO
            }
        }

        public bool TryGetValue(byte slotId, out Item value)
        {
            return _items.TryGetValue(slotId, out value);
        }

        public void Reset()
        {
            // TODO
        }

        public void Reset(ICollection<Item> items)
        {
            // TODO
        }

        public void Reset(IEnumerable<Item> items)
        {
            // TODO
        }

        public void Reset(IDictionary<byte, Item> item)
        {
            // TODO
        }

        public void Reset(ICollection<KeyValuePair<byte, Item>> item)
        {
            // TODO
        }

        public void Reset(IEnumerable<KeyValuePair<byte, Item>> item)
        {
            // TODO
        }

        public void Clear()
        {
            Reset();
        }

        public bool Contains(Item item)
        {
            // TODO

            return false;
        }

        public bool Contains(KeyValuePair<byte, Item> item)
        {
            // TODO

            return false;
        }

        public bool Add(Item item)
        {
            // TODO

            return false;
        }

        public bool Add(byte slotId, Item item)
        {
            // TODO

            return false;
        }

        public bool Add(KeyValuePair<byte, Item> item)
        {
            return Add(item.Key, item.Value);
        }

        public bool Remove(byte slotId)
        {
            // TODO

            return false;
        }

        public bool Remove(KeyValuePair<byte, Item> item)
        {
            // TODO

            return false;
        }

        public void CopyTo(KeyValuePair<byte, Item>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<byte, Item>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        void ICollection<KeyValuePair<byte, Item>>.Add(KeyValuePair<byte, Item> item)
        {
            this.Add(item);
        }

        void IDictionary<byte, Item>.Add(byte slotId, Item item)
        {
            this.Add(slotId, item);
        }

        private bool ContainsKey(byte slotId)
        {
            return slotId >= 0 &&
                   slotId <= MaxSlotsCount;
        }

        bool IDictionary<byte, Item>.ContainsKey(byte slotId)
        {
            return ContainsKey(slotId);
        }

        bool IReadOnlyDictionary<byte, Item>.ContainsKey(byte slotId)
        {
            return ContainsKey(slotId);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}