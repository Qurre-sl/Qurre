using InventorySystem.Disarming;
using MapGeneration;
using System.Linq;
using UnityEngine;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    using Qurre.API.Controllers;
    using Qurre.API.Objects;

    public sealed class GamePlay
    {
        private readonly Player _player;
        internal GamePlay(Player pl)
        {
            _player = pl;
            BlockSpawnTeleport = false;
        }

        public bool BlockSpawnTeleport { get; set; }

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
                    if (DisarmedPlayers.Entries[i].DisarmedPlayer == _player.Inventory.Base.netId)
                    {
                        DisarmedPlayers.Entries.RemoveAt(i);
                        break;
                    }
                }

                if (value != null)
                    _player.Inventory.Base.SetDisarmedStatus(value.Inventory.Base);
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