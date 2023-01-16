using Qurre.API;
using Qurre.API.Attributes;
using Qurre.Events;
using Qurre.Events.Structs;

namespace Qurre.Internal.EventsCalled
{
    // delete later
    static class Testing
    {
        [EventMethod(PlayerEvents.PickupArmor)]
        [EventMethod(PlayerEvents.PickupItem)]
        static internal void TestMultiple(IBaseEvent ev)
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
        static internal void Test(EscapeEvent ev)
        {
            Log.Info($"Pl: {ev.Player?.UserInfomation.Nickname}; Role: {ev.Role};");
            //ev.Role = PlayerRoles.RoleTypeId.Tutorial;
        }

        [EventMethod(PlayerEvents.Spawn)]
        static internal void Test(SpawnEvent ev)
        {
            var item = ev.Player.Inventory.AddItem(ItemType.SCP1576);
            MEC.Timing.CallDelayed(15f, () => ev.Player.Inventory.DropItem(item));
            MEC.Timing.CallDelayed(25f, () => ev.Player.Inventory.DropAll());
            MEC.Timing.CallDelayed(35f, () => ev.Player.Inventory.AddItem(ItemType.Coin, 5));
            MEC.Timing.CallDelayed(45f, () => ev.Player.Inventory.Clear());
            Log.Info($"Pl: {ev.Player?.UserInfomation.Nickname}; Role: {ev.Role};");
            //ev.Role = PlayerRoles.RoleTypeId.Tutorial;
        }
    }
}