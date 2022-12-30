using InventorySystem;
using Mirror;
using System.Collections.Generic;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    using Qurre.API.Controllers;
    using Qurre.API.Objects;

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