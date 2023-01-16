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

        public InventorySystem.Inventory Inventory => _player.ReferenceHub.inventory;

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
    }
}