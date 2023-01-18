using System;
using PlayerRoles;
using Qurre.API.Attributes;
using Qurre.Events;
using Qurre.Events.Structs;

namespace Qurre.Internal.EventsCalled
{
    internal static class Player
    {
        [EventMethod(PlayerEvents.Join)]
        internal static void JoinLog(JoinEvent ev)
            => ServerConsole.AddLog(
                $"Player {ev.Player?.UserInfomation.Nickname} ({ev.Player?.UserInfomation.UserId}) " + $"({ev.Player?.UserInfomation.Id}) connected. iP: {ev.Player?.UserInfomation.Ip}",
                ConsoleColor.Magenta);

        [EventMethod(PlayerEvents.Spawn)]
        internal static void UpdateRole(SpawnEvent ev)
        {
            switch (ev.Role)
            {
                case RoleTypeId.Scp079:
                {
                    ev.Player.RoleInfomation.Scp079 = new (ev.Player);
                    break;
                }
            }
        }
    }
}