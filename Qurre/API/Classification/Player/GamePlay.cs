using InventorySystem.Disarming;
using JetBrains.Annotations;
using MapGeneration;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using Qurre.API.World;
using UnityEngine;

namespace Qurre.API.Classification.Player;

[PublicAPI]
public sealed class GamePlay
{
    private readonly Controllers.Player _player;

    internal GamePlay(Controllers.Player pl)
    {
        _player = pl;
        BlockSpawnTeleport = false;
    }

    public bool BlockSpawnTeleport { get; set; }

    public bool Cuffed => _player.ReferenceHub.inventory.IsDisarmed();

    public FacilityZone CurrentZone => Room.Zone;

    public bool Overwatch
    {
        get => _player.ReferenceHub.serverRoles.IsInOverwatch;
        set => _player.ReferenceHub.serverRoles.IsInOverwatch = value;
    }

    public bool GodMode
    {
        get => _player.ClassManager.GodMode;
        set => _player.ClassManager.GodMode = value;
    }

    public Room Room
    {
        get => LabApi.Features.Wrappers.Player.Get(_player.ReferenceHub).Room?.Base.GetRoom() ??
               Map.Rooms.Find(x => x.Type == RoomType.Surface);
        set => _player.MovementState.Position = value.Position + Vector3.up * 2;
    }

    public Lift? Lift
    {
        get => _player.MovementState.Position.GetLift();
        set
        {
            if (value == null)
                return;

            _player.MovementState.Position = value.Position + Vector3.up * 2;
        }
    }

    public Controllers.Player? Cuffer
    {
        get
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (DisarmedPlayers.DisarmedEntry disarmed in DisarmedPlayers.Entries)
                if (disarmed.DisarmedPlayer == _player.ReferenceHub.netId)
                    return disarmed.Disarmer.GetPlayer();

            return null;
        }
        set
        {
            for (int i = 0; i < DisarmedPlayers.Entries.Count; i++)
                if (DisarmedPlayers.Entries[i].DisarmedPlayer == _player.Inventory.Base.netId)
                {
                    DisarmedPlayers.Entries.RemoveAt(i);
                    break;
                }

            if (value != null)
                _player.Inventory.Base.SetDisarmedStatus(value.Inventory.Base);
        }
    }
}