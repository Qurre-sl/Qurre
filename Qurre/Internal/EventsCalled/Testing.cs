/*
using Qurre.API;
using Qurre.API.Attributes;
using Qurre.API.Objects;
using Qurre.Events;
using Qurre.Events.Structs;
using System.Collections.Generic;
using UnityEngine;

namespace Qurre.Internal.EventsCalled
{
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

        [EventMethod(RoundEvents.Waiting)]
        static internal void LogDoors()
        {
            MEC.Timing.CallDelayed(1f, () =>
            {
                Log.Info("----- Logging Doors -----");
                List<string> names = new();
                foreach (var door in Map.Doors)
                {
                    if (!names.Contains(door.Name))
                    {
                        names.Add(door.Name);
                        string str = $"Door: '{door.Name}' > '{door.Type}'; Rooms: ";
                        foreach (var room in door.Rooms)
                            str += $"'{room.RoomName} ({room.Name})'; ";
                        Log.Info(str);
                    }
                }
                Log.Info("----- Unknown doors -----");
                foreach (var door in Map.Doors)
                {
                    if (door.Type == DoorType.Unknown)
                    {
                        string str = $"Door: '{door.Name}' > '{door.Type}'; Rooms: ";
                        foreach (var room in door.Rooms)
                            str += $"'{room.RoomName} ({room.Name})'; ";
                        Log.Info(str);
                    }
                }
                Log.Info("----- End log -----");
            });
        }

        [EventMethod(PlayerEvents.InteractDoor)]
        static internal void Test(InteractDoorEvent ev)
        {
            string rms = string.Empty;
            foreach (var room in ev.Door.Rooms)
                rms += $"'{room.RoomName} ({room.Name})'; ";
            Log.Info($"Pl: {ev.Player?.UserInfomation.Nickname}; Type: {ev.Door.Type}; Name: {ev.Door.Name}; Rooms: {rms}");
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

        [EventMethod(PlayerEvents.Join)]
        static void TestCmdSync(JoinEvent ev)
        {
            ev.Player.ClassManager.TargetChangeCmdBinding(KeyCode.LeftAlt, ".test");
        }
    }
}
*/