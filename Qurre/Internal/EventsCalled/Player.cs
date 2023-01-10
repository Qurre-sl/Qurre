using Qurre.API;
using Qurre.API.Attributes;
using Qurre.Events;
using Qurre.Events.Structs;
using RoundRestarting;
using System;

namespace Qurre.Internal.EventsCalled
{
    static class Player
    {
        [EventMethod(PlayerEvents.Join)]
        static internal void JoinLog(JoinEvent ev)
        {
            ServerConsole.AddLog($"Player {ev.Player?.UserInfomation.Nickname} ({ev.Player?.UserInfomation.UserId}) " +
                $"({ev.Player?.UserInfomation.Id}) connected. iP: {ev.Player?.UserInfomation.Ip}", ConsoleColor.Magenta);
        }

        [EventMethod(PlayerEvents.PickupArmor)]
        [EventMethod(PlayerEvents.PickupItem)]
        static internal void TestMultiple(IBaseEvent ev)
        {
            if (ev is PickupArmorEvent ev1)
            {
                API.Log.Info($"Armor; Pl: {ev1.Player?.UserInfomation.Nickname}; Item: {ev1.Pickup?.Serial}");
                //ev1.Allowed = false;
            }
            else if (ev is PickupItemEvent ev2)
            {
                API.Log.Info($"Item; Pl: {ev2.Player?.UserInfomation.Nickname}; Item: {ev2.Pickup?.Serial}");
                //ev2.Allowed = false;
            }
        }

        [EventMethod(PlayerEvents.Spawn)]
        static internal void Test(SpawnEvent ev)
        {
            API.Log.Info($"Spawn; Pl: {ev.Player?.UserInfomation.Nickname}; Role: {ev.Role}; Position: {ev.Position}; Rot: {ev.Rotation}");
            //ev.Position = UnityEngine.Vector3.zero;
        }
    }
}