using PlayerRoles;
using Qurre.API;
using Qurre.API.Addons;
using Qurre.API.Attributes;
using Qurre.API.Classification.Roles;
using Qurre.Events;
using Qurre.Events.Structs;
using System;

#pragma warning disable IDE0051
namespace Qurre.Internal.EventsCalled
{
    static class Player
    {
        [EventMethod(PlayerEvents.Join)]
        static void Join(JoinEvent ev)
        {
            ServerConsole.AddLog($"Player {ev.Player?.UserInformation.Nickname} ({ev.Player?.UserInformation.UserId}) " +
                $"({ev.Player?.UserInformation.Id}) connected. iP: {ev.Player?.UserInformation.Ip}", ConsoleColor.Magenta);

            // send net identity info
            foreach (var door in Map.Doors)
            {
                if (!door.Custom)
                    continue;

                try
                {
                    door.DoorVariant.netIdentity.UpdateDataForConnection(ev.Player.ConnectionToClient);
                }
                catch (Exception ex)
                {
                    Log.Debug($"Error in {BetterColors.Yellow("Qurre.Internal.EventsCalled.Join<DoorsUpdate>")}\n{BetterColors.Grey(ex)}");
                }
            }

            foreach (var locker in Map.Lockers)
            {
                if (!locker.Custom)
                    continue;

                try
                {
                    locker._locker.netIdentity.UpdateDataForConnection(ev.Player.ConnectionToClient);
                }
                catch (Exception ex)
                {
                    Log.Debug($"Error in {BetterColors.Yellow("Qurre.Internal.EventsCalled.Join<LockersUpdate>")}\n{BetterColors.Grey(ex)}");
                }
            }
        }

        [EventMethod(PlayerEvents.Leave)]
        static void LeaveClears(LeaveEvent ev)
        {
            if (Scp173.IgnoredPlayers.Contains(ev.Player))
                Scp173.IgnoredPlayers.Remove(ev.Player);
        }

        [EventMethod(PlayerEvents.Spawn)]
        static void BlockSpawnTeleport(SpawnEvent ev)
        {
            if (!ev.Player.GamePlay.BlockSpawnTeleport)
                return;

            ev.Position = ev.Player.MovementState.Position;
        }

        [EventMethod(PlayerEvents.Spawn)]
        static void SetMaxHp(SpawnEvent ev)
        {
            if (ev.Player.ReferenceHub.roleManager.CurrentRole is IHealthbarRole healthbarRole)
                ev.Player.HealthInformation.MaxHp = healthbarRole.MaxHealth;
            else
                ev.Player.HealthInformation.MaxHp = 0;

            ev.Player.HealthInformation.Hp = ev.Player.HealthInformation.MaxHp;
        }

        [EventMethod(PlayerEvents.Spawn)]
        static void UpdateRole(SpawnEvent ev)
        {
            if (ev.Role is not RoleTypeId.Spectator and not RoleTypeId.None and not RoleTypeId.Overwatch)
                ev.Player.RoleInformation.cachedRole = ev.Role;
            switch (ev.Role)
            {
                case RoleTypeId.Scp079:
                    {
                        ev.Player.RoleInformation.Scp079 = new(ev.Player);
                        break;
                    }
                case RoleTypeId.Scp096:
                    {
                        ev.Player.RoleInformation.Scp096 = new(ev.Player);
                        break;
                    }
                case RoleTypeId.Scp106:
                    {
                        ev.Player.RoleInformation.Scp106 = new(ev.Player);
                        break;
                    }
                case RoleTypeId.Scp173:
                    {
                        ev.Player.RoleInformation.Scp173 = new(ev.Player);
                        break;
                    }
                default: break;
            }
        }

        [EventMethod(PlayerEvents.ChangeRole, int.MinValue)]
        static void SetSpawnedTime(ChangeRoleEvent ev)
        {
            if (!ev.Allowed)
                return;

            if (ev.Role is RoleTypeId.Spectator or RoleTypeId.Overwatch or RoleTypeId.Filmmaker)
                return;

            ev.Player.SpawnedTime = DateTime.Now;
        }
    }
}
#pragma warning restore IDE0051