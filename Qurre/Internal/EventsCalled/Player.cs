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
            ServerConsole.AddLog($"Player {ev.Player?.UserInfomation.NickName} ({ev.Player?.UserInfomation.UserId}) " +
                $"({ev.Player?.UserInfomation.Id}) connected. iP: {ev.Player?.UserInfomation.Ip}", ConsoleColor.Magenta);
        }

        [EventMethod(PlayerEvents.Attack)]
        static internal void Waiting(AttackEvent ev)
        {
            API.Log.Info($"attacker: {ev.Attacker.UserInfomation.NickName}; target: {ev.Target.UserInfomation.NickName}; " +
                $"Amount: {ev.Damage}; DamageType: {ev.DamageType}; LiteDamage: {ev.LiteType}; FF: {ev.FriendlyFire}");
            ev.Damage = 1;
        }
    }
}