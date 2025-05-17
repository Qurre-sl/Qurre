using System.Collections.Generic;
using JetBrains.Annotations;
using Qurre.API.Objects;

namespace Qurre.API.Classification.Structs;

[PublicAPI]
public sealed class AmmoBox
{
    private readonly Controllers.Player _player;

    internal AmmoBox(Controllers.Player player)
    {
        _player = player;
    }

    public ushort Ammo12Gauge
    {
        get => this[AmmoType.Ammo12Gauge];
        set => this[AmmoType.Ammo12Gauge] = value;
    }

    public ushort Ammo556
    {
        get => this[AmmoType.Ammo556];
        set => this[AmmoType.Ammo556] = value;
    }

    public ushort Ammo44Cal
    {
        get => this[AmmoType.Ammo44Cal];
        set => this[AmmoType.Ammo44Cal] = value;
    }

    public ushort Ammo762
    {
        get => this[AmmoType.Ammo762];
        set => this[AmmoType.Ammo762] = value;
    }

    public ushort Ammo9
    {
        get => this[AmmoType.Ammo9];
        set => this[AmmoType.Ammo9] = value;
    }

    public ushort this[AmmoType ammo]
    {
        get => _player.Inventory.Base.UserInventory.ReserveAmmo.GetValueOrDefault(ammo.GetItemType(), (ushort)0);
        set
        {
            _player.Inventory.Base.UserInventory.ReserveAmmo[ammo.GetItemType()] = value;
            _player.Inventory.Base.SendAmmoNextFrame = true;
        }
    }
}