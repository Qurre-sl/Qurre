namespace Qurre.API.Classification.Player
{
    using InventorySystem;
    using Mirror;
    using Qurre.API;
    using Qurre.API.Controllers;
    using Qurre.API.Objects;
    using System.Collections.Generic;

    public sealed class InventoryInformation
    {
        public Inventory Base { get; }

        public InventoryItems Items;

        public InventoryInformation(Player player)
        {
            Base = player.ReferenceHub.inventory;
            Items = new InventoryItems(Base);
        }
    }
}
