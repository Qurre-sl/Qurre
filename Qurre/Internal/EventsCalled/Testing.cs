using MEC;
using Qurre.API;
using Qurre.API.Attributes;
using Qurre.API.Controllers;
using Qurre.Events;
using Qurre.Events.Structs;

namespace Qurre.Internal.EventsCalled
{
    // delete later
    internal static class Testing
    {
        [EventMethod(PlayerEvents.PickupArmor)]
        [EventMethod(PlayerEvents.PickupItem)]
        internal static void TestMultiple(IBaseEvent ev)
        {
            if (ev is PickupArmorEvent ev1)
            {
                Log.Info($"Armor; Pl: {ev1.Player?.UserInfomation.Nickname}; Item: {ev1.Pickup?.Serial}");
                //ev1.Allowed = false;
            }
            else if (ev is PickupItemEvent ev2)
            {
                Log.Info($"Item; Pl: {ev2.Player?.UserInfomation.Nickname}; Item: {ev2.Pickup?.Serial}");
                //ev2.Allowed = false;
            }
        }

        [EventMethod(PlayerEvents.Escape)]
        internal static void Test(EscapeEvent ev)
            => Log.Info($"Pl: {ev.Player?.UserInfomation.Nickname}; Role: {ev.Role};");

        //ev.Role = PlayerRoles.RoleTypeId.Tutorial;
        [EventMethod(PlayerEvents.Spawn)]
        internal static void Test(SpawnEvent ev)
        {
            Item item = ev.Player.Inventory.AddItem(ItemType.SCP1576);
            Timing.CallDelayed(15f, () => ev.Player.Inventory.DropItem(item));
            Timing.CallDelayed(25f, () => ev.Player.Inventory.DropAll());
            Timing.CallDelayed(35f, () => ev.Player.Inventory.AddItem(ItemType.Coin, 5));
            Timing.CallDelayed(45f, () => ev.Player.Inventory.Clear());
            Log.Info($"Pl: {ev.Player?.UserInfomation.Nickname}; Role: {ev.Role};");
            //ev.Role = PlayerRoles.RoleTypeId.Tutorial;
        }
    }
}