using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.API.Classification.Player
{
    using InventorySystem.Disarming;
    using InventorySystem;
    using Qurre.API;
    public class GamePlay
    {
        private readonly Player _player;
        internal GamePlay(Player pl) => _player = pl;

        public Inventory Inventory => _player.ReferenceHub.inventory;
        public bool IsDisarmed => _player.ReferenceHub.inventory.IsDisarmed();
        public bool Overwatch
        {
            get => _player.ReferenceHub.serverRoles.OverwatchEnabled;
            set => _player.ReferenceHub.serverRoles.SetOverwatchStatus(value);
        }
    }
}
