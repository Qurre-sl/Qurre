using System;
using PlayerRoles;
using Qurre.API;
using Qurre.API.Addons;
using Qurre.API.Attributes;
using Qurre.API.Classification.Roles;
using Qurre.API.Controllers;
using Qurre.API.World;
using Qurre.Events;
using Qurre.Events.Structs;

namespace Qurre.Internal.EventsCalled;

internal static class Player
{
    [EventMethod(PlayerEvents.Join)]
    private static void Join(JoinEvent ev)
    {
        ServerConsole.AddLog($"Player {ev.Player.UserInformation.Nickname} ({ev.Player.UserInformation.UserId}) " +
                             $"({ev.Player.UserInformation.Id}) connected. iP: {ev.Player.UserInformation.Ip}",
            ConsoleColor.Magenta);

        /* --- send net identity info --- */

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (Door? door in Map.Doors)
        {
            if (!door.Custom)
                continue;

            try
            {
                door.DoorVariant.netIdentity.UpdateDataForConnection(ev.Player.ConnectionToClient);
            }
            catch (Exception ex)
            {
                Log.Debug(
                    $"Error in {BetterColors.Yellow("Qurre.Internal.EventsCalled.Join<DoorsUpdate>")}\n{BetterColors.Grey(ex)}");
            }
        }

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (Locker? locker in Map.Lockers)
        {
            if (!locker.Custom)
                continue;

            try
            {
                locker.GlobalLocker.netIdentity.UpdateDataForConnection(ev.Player.ConnectionToClient);
            }
            catch (Exception ex)
            {
                Log.Debug(
                    $"Error in {BetterColors.Yellow("Qurre.Internal.EventsCalled.Join<LockersUpdate>")}\n{BetterColors.Grey(ex)}");
            }
        }
    }

    [EventMethod(PlayerEvents.Leave)]
    private static void LeaveClears(LeaveEvent ev)
    {
        if (Scp173.IgnoredPlayers.Contains(ev.Player))
            Scp173.IgnoredPlayers.Remove(ev.Player);
    }

    [EventMethod(PlayerEvents.Spawn)]
    private static void BlockSpawnTeleport(SpawnEvent ev)
    {
        if (!ev.Player.GamePlay.BlockSpawnTeleport)
            return;

        ev.Position = ev.Player.MovementState.Position;
    }

    [EventMethod(PlayerEvents.Spawn)]
    private static void SetMaxHp(SpawnEvent ev)
    {
        if (ev.Player.ReferenceHub.roleManager.CurrentRole is IHealthbarRole healthRole)
            ev.Player.HealthInformation.MaxHp = healthRole.MaxHealth;
        else
            ev.Player.HealthInformation.MaxHp = 0;

        ev.Player.HealthInformation.Hp = ev.Player.HealthInformation.MaxHp;
    }

    [EventMethod(PlayerEvents.Spawn)]
    private static void UpdateRole(SpawnEvent ev)
    {
        if (ev.Role is not RoleTypeId.Spectator and not RoleTypeId.None and not RoleTypeId.Overwatch)
            ev.Player.RoleInformation.CachedRole = ev.Role;

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (ev.Role)
        {
            case RoleTypeId.Scp079:
                {
                    ev.Player.RoleInformation.Scp079 = new Scp079(ev.Player);
                    break;
                }
            case RoleTypeId.Scp096:
                {
                    ev.Player.RoleInformation.Scp096 = new Scp096(ev.Player);
                    break;
                }
            case RoleTypeId.Scp106:
                {
                    ev.Player.RoleInformation.Scp106 = new Scp106(ev.Player);
                    break;
                }
            case RoleTypeId.Scp173:
                {
                    ev.Player.RoleInformation.Scp173 = new Scp173(ev.Player);
                    break;
                }
        }
    }

    [EventMethod(PlayerEvents.ChangeRole, int.MinValue)]
    private static void SetSpawnedTime(ChangeRoleEvent ev)
    {
        if (!ev.Allowed)
            return;

        if (ev.Role is RoleTypeId.Spectator or RoleTypeId.Overwatch or RoleTypeId.Filmmaker)
            return;

        ev.Player.SpawnedTime = DateTime.Now;
    }

    [EventMethod(PlayerEvents.Dead, int.MinValue)]
    private static void Dead(DeadEvent ev)
    {
        ev.Target.StatsInformation.DeathsCount++;

        if (ev.Target == ev.Attacker)
            return;

        ev.Attacker.StatsInformation.LocalKills.Add(
            new KillElement(ev.Attacker, ev.Target, ev.DamageType, DateTime.Now));
    }
}