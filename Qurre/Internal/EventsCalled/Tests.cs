#if TESTS
using System;
using System.Linq;
using Mirror;
using Qurre.API;
using Qurre.API.Addons;
using Qurre.API.Attributes;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using Qurre.API.World;
using Qurre.Events;
using Qurre.Events.Structs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.Internal.EventsCalled;

internal static class Tests
{
    [EventMethod(RoundEvents.Waiting)]
    internal static void Waiting()
    {
        #region Rooms

        Log.Info("----- ROOMS -----");

        foreach (Room room in Map.Rooms)
        {
            if (room.Type != RoomType.Unknown)
                Log.Debug($"{room.Name}: {room.Type}");
            else
                Log.Warn($"{room.Name}: {room.Type}");

            room.Position = Vector3.zero;

            Log.Custom(room.NetworkIdentity
                    ? $"Room {room.Name} has a network identity."
                    : $"Room {room.Name} {BetterColors.Red("not have")} network identity",
                "TESTS: ROOMS: NETWORK");
        }

        Log.Custom("----------------------------------------------");

        foreach (object? roomType in Enum.GetValues(typeof(RoomType)))
        {
            if (Map.Rooms.Exists(x => $"{x.Type}" == $"{roomType}"))
            {
                Log.Debug($"Room \"{roomType}\" exist");
                continue;
            }

            Log.Warn($"Room \"{roomType}\" does not exist");
        }

        Log.Info("----- ROOMS -----");

        #endregion

        #region Doors

        Log.Info("----- DOORS -----");

        foreach (Door door in Map.Doors)
            if (door.Type != DoorType.Unknown)
                Log.Debug($"{door.Name}: {door.Type}");
            else
                Log.Warn($"{door.Name}: {door.Type}");

        Log.Custom("----------------------------------------------");

        foreach (object? doorType in Enum.GetValues(typeof(DoorType)))
        {
            if (Map.Doors.Exists(x => $"{x.Type}" == $"{doorType}"))
            {
                Log.Debug($"Door \"{doorType}\" exist");
                continue;
            }

            Log.Warn($"Door \"{doorType}\" does not exist");
        }

        Log.Info("----- DOORS -----");

        #endregion
    }

    [EventMethod(ServerEvents.GameConsoleCommand)]
    internal static void GetRoom(GameConsoleCommandEvent ev)
    {
        switch (ev.Name)
        {
            case "room_get":
            {
                Room room = ev.Player.GamePlay.Room;

                ev.Allowed = false;
                ev.Reply = $"Name: {room.Name}; RoomName: {room.RoomName}; Type: {room.Type}";

                break;
            }

            case "room_tp":
            {
                ev.Allowed = false;

                string roomName = string.Join(' ', ev.Args);
                Room? room = Map.Rooms.Find(x => x.Name == roomName);

                if (room is null)
                {
                    ev.Reply = $"Room \"{roomName}\" does not exist";
                    break;
                }

                ev.Player.MovementState.Position = room.Position + Vector3.up;

                break;
            }

            case "spawn_lift":
            {
                ev.Allowed = false;
                GameObject? prefab = NetworkClient.prefabs.First(x => x.Key == 2588580243).Value;
                GameObject? pref = Object.Instantiate(prefab);
                pref.transform.position = ev.Player.MovementState.Position;
                NetworkServer.Spawn(pref);
                break;
            }

            case "room_move":
            {
                ev.Allowed = false;
                ev.Reply = $"{ev.Player.GamePlay.Room.NetworkIdentity}";
                ev.Player.GamePlay.Room.Position = Vector3.zero;
                break;
            }
        }
    }
}
#endif