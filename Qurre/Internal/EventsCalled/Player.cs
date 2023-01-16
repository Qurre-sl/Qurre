using Qurre.API;
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

        [EventMethod(PlayerEvents.Spawn)]
        static internal void UpdateRole(SpawnEvent ev)
        {
            switch (ev.Role)
            {
                case PlayerRoles.RoleTypeId.Scp079:
                    {
                        ev.Player.RoleInfomation.Scp079 = new(ev.Player);
                        break;
                    }
                default: break;
            }
        }
    }
}