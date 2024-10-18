using InventorySystem.Items.ToggleableLights;
using InventorySystem.Items.ToggleableLights.Flashlight;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using Utils.Networking;

namespace Qurre.API.Addons.Items;

[PublicAPI]
public sealed class Flashlight(FlashlightItem itemBase) : Item(itemBase)
{
    private const ItemType FlashlightItemType = ItemType.Flashlight;

    public Flashlight() : this((FlashlightItem)FlashlightItemType.CreateItemInstance())
    {
    }

    public FlashlightItem GameBase { get; } = itemBase;

    public bool Active
    {
        get => GameBase.IsEmittingLight;
        set
        {
            GameBase.IsEmittingLight = value;
            new FlashlightNetworkHandler.FlashlightMessage(Serial, value).SendToAuthenticated();
        }
    }
}