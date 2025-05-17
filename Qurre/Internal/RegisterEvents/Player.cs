using InventorySystem.Items.Usables.Scp330;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Qurre.API;
using Qurre.API.Entities.Items.Implementations;
using Qurre.Events.Structs;
using Qurre.Internal.Attributes;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.RegisterEvents;

internal static class Player
{
    [SelfInvoke]
    internal static void Init()
    {
        PlayerEvents.SearchedPickup += OnSearchedPickup;
        PlayerEvents.PickingUpScp330 += OnPickingUpScp330;
        PlayerEvents.InteractingLocker += OnInteractingLocker;
    }

    private static void OnSearchedPickup(PlayerSearchedPickupEventArgs labApiEv)
    {
        var player = labApiEv.Player.ReferenceHub.GetPlayer();
        var pickup = labApiEv.Pickup.Base;

        if (pickup is Scp330Pickup scp330Pickup)
        {
            scp330Pickup.
        }
    }

    private static void OnPickingUpScp330(PlayerPickingUpScp330EventArgs labApiEv)
    {
        var player = labApiEv.Player.ReferenceHub.GetPlayer();
        
        var qurreEv = new PickupCandyEvent(player, bag, candyList);
    }

    private static void OnInteractingLocker(PlayerInteractingLockerEventArgs labApiEv)
    {
        var player = labApiEv.Player.ReferenceHub.GetPlayer();
        var locker = labApiEv.Locker.Base.GetLocker();

        if (player == null)
        {
            Log.Error("[Qurre.Internal.RegisterEvents.Player] Player is null.");
            return;
        }
        if (locker == null)
        {
            Log.Error("[Qurre.Internal.RegisterEvents.Player] Locker is null.");
            return;
        }

        locker.Chambers.TryGet(labApiEv.Chamber.Id, out var chamber);
        
        var qurreEv = new InteractLockerEvent(player, locker, chamber, labApiEv.IsAllowed);
        qurreEv.InvokeEvent();
        
        labApiEv.IsAllowed = qurreEv.IsAllowed;
        labApiEv.CanOpen = qurreEv.CanOpen;
    }
}