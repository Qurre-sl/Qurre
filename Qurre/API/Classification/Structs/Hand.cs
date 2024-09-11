using InventorySystem.Items;
using JetBrains.Annotations;
using Qurre.API.Controllers;

namespace Qurre.API.Classification.Structs;

[PublicAPI]
public sealed class Hand
{
    private readonly API.Player _player;

    internal Hand(API.Player player)
    {
        _player = player;
    }

    public bool IsEmpty =>
        _player.Inventory.Base.CurItem.TypeId == ItemType.None;

    public ItemType Type =>
        _player.Inventory.Base.CurItem.TypeId;

    public ushort Serial
        => _player.Inventory.Base.CurItem.SerialNumber;

    public ItemBase? ItemBase
        => _player.Inventory.Base.CurInstance;

    public Item? Item
        => Item.Get(_player.Inventory.Base.CurInstance);
}