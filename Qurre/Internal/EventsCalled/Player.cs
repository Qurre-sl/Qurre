using Qurre.API.Attributes;
using Qurre.Events;
using Qurre.Events.Structs;
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

        [EventMethod(PlayerEvents.ThrowProjectile)]
        static internal void Test(ThrowProjectileEvent ev)
        {
            API.Log.Info($"Throw; Pl: {ev.Player?.UserInfomation.Nickname}; Item: {ev.Item?.Serial}");
            //ev.Allowed = false;
        }

        [EventMethod(PlayerEvents.DropAmmo)]
        static internal void Test(DropAmmoEvent ev)
        {
            API.Log.Info($"Drop; Pl: {ev.Player?.UserInfomation.Nickname}; Type: {ev.Type}; Amount: {ev.Amount}");
            //ev.Allowed = false;
        }
    }
}