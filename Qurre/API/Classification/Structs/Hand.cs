using InventorySystem.Items;
using JetBrains.Annotations;

namespace Qurre.API.Classification.Structs;

[PublicAPI]
public sealed class Hand
{
    private readonly Controllers.Player _player;

    internal Hand(Controllers.Player player)
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

    public ItemBase? Item
        => _player.Inventory.Base.CurInstance;
}