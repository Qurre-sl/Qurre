namespace Qurre.API.Classification.Structs
{
    using InventorySystem.Items;
    using Qurre.API;
    using Qurre.API.Controllers;

    public sealed class Hand
    {
        readonly Player _player;

        public bool IsEmpty =>
            _player.Inventory.Base.CurItem.TypeId == ItemType.None;

        public ItemType Type =>
            _player.Inventory.Base.CurItem.TypeId;

        public ushort Serial
            => _player.Inventory.Base.CurItem.SerialNumber;

        public ItemBase ItemBase
            => _player.Inventory.Base.CurInstance;

        public Item Item
            => Item.Get(_player.Inventory.Base.CurInstance);

        internal Hand(Player player)
        {
            _player = player;
        }
    }
}