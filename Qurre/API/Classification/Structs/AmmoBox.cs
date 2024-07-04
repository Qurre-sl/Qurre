namespace Qurre.API.Classification.Structs
{
	using Qurre.API;
	using Qurre.API.Objects;

	public class AmmoBox
	{
		readonly Player _player;
		internal AmmoBox(Player player)
		{
			_player = player;
		}

		public ushort Ammo12Gauge
		{
			get { try { return this[AmmoType.Ammo12Gauge]; } catch { return 0; } }
			set { try { this[AmmoType.Ammo12Gauge] = value; } catch { } }
		}
		public ushort Ammo556
		{
			get { try { return this[AmmoType.Ammo556]; } catch { return 0; } }
			set { try { this[AmmoType.Ammo556] = value; } catch { } }
		}
		public ushort Ammo44Cal
		{
			get { try { return this[AmmoType.Ammo44Cal]; } catch { return 0; } }
			set { try { this[AmmoType.Ammo44Cal] = value; } catch { } }
		}
		public ushort Ammo762
		{
			get { try { return this[AmmoType.Ammo762]; } catch { return 0; } }
			set { try { this[AmmoType.Ammo762] = value; } catch { } }
		}
		public ushort Ammo9
		{
			get { try { return this[AmmoType.Ammo9]; } catch { return 0; } }
			set { try { this[AmmoType.Ammo9] = value; } catch { } }
		}

		public ushort this[AmmoType ammo]
		{
			get
			{
				if (_player.Inventory.Base.UserInventory.ReserveAmmo.TryGetValue(ammo.GetItemType(), out ushort amount))
					return amount;

				return 0;
			}
			set
			{
				_player.Inventory.Base.UserInventory.ReserveAmmo[ammo.GetItemType()] = value;
				_player.Inventory.Base.SendAmmoNextFrame = true;
			}
		}
	}
}