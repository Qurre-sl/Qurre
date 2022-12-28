using InventorySystem.Disarming;
using InventorySystem;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    public class GamePlay
    {
        private readonly Player _player;
        internal GamePlay(Player pl) => _player = pl;

        public Inventory Inventory => _player.ReferenceHub.inventory;

        public bool Cuffed => _player.ReferenceHub.inventory.IsDisarmed();
        public bool Overwatch
        {
            get => _player.ReferenceHub.serverRoles.OverwatchEnabled;
            set => _player.ReferenceHub.serverRoles.SetOverwatchStatus(value);
        }
    }
}