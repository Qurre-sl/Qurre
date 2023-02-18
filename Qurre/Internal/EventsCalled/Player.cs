using PlayerRoles;
using Qurre.API;
using Qurre.API.Attributes;
using Qurre.API.Classification.Roles;
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

        [EventMethod(PlayerEvents.Leave)]
        static internal void LeaveClears(LeaveEvent ev)
        {
            if (Scp173.IgnoredPlayers.Contains(ev.Player))
                Scp173.IgnoredPlayers.Remove(ev.Player);
        }

        [EventMethod(PlayerEvents.Spawn)]
        static internal void BlockSpawnTeleport(SpawnEvent ev)
        {
            if (!ev.Player.GamePlay.BlockSpawnTeleport)
                return;

            ev.Position = ev.Player.MovementState.Position;
        }

        [EventMethod(PlayerEvents.Spawn)]
        static internal void SetMaxHp(SpawnEvent ev)
        {
            if (ev.Player.ReferenceHub.roleManager.CurrentRole is IHealthbarRole healthbarRole)
                ev.Player.HealthInfomation.MaxHp = healthbarRole.MaxHealth;
            else
                ev.Player.HealthInfomation.MaxHp = 0;

            ev.Player.HealthInfomation.Hp = ev.Player.HealthInfomation.MaxHp;
        }

        [EventMethod(PlayerEvents.Spawn)]
        static internal void UpdateRole(SpawnEvent ev)
        {
            if (ev.Role is not RoleTypeId.Spectator and not RoleTypeId.None and not RoleTypeId.Overwatch)
                ev.Player.RoleInfomation.cachedRole = ev.Role;
            switch (ev.Role)
            {
                case RoleTypeId.Scp079:
                    {
                        ev.Player.RoleInfomation.Scp079 = new(ev.Player);
                        break;
                    }
                case RoleTypeId.Scp106:
                    {
                        ev.Player.RoleInfomation.Scp106 = new(ev.Player);
                        break;
                    }
                case RoleTypeId.Scp173:
                    {
                        ev.Player.RoleInfomation.Scp173 = new(ev.Player);
                        break;
                    }
                default: break;
            }
        }
    }
}