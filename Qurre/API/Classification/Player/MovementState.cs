using System;
using System.Reflection;
using JetBrains.Annotations;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace Qurre.API.Classification.Player;

[PublicAPI]
public sealed class MovementState
{
    private readonly Controllers.Player _player;

    internal MovementState(Controllers.Player pl)
    {
        _player = pl;
    }

    public Transform CameraReference => _player.ReferenceHub.PlayerCameraReference;
    public Transform Transform => _player.ReferenceHub.transform;

    public Vector3 Position
    {
        get => _player.GameObject.transform.position;
        set => _player.ReferenceHub.TryOverridePosition(value);
    }

    public Vector3 Rotation
    {
        get => _player.GameObject.transform.eulerAngles;
        set => _player.ReferenceHub.TryOverrideRotation(value);
    }

    public Vector3 Scale
    {
        get => _player.ReferenceHub.transform.localScale;
        set
        {
            try
            {
                if (_player.Disconnected)
                {
                    Log.Debug(
                        $"Scale: Player already disconnected. Called from: {Assembly.GetCallingAssembly().GetName().Name}");
                    return;
                }

                _player.ReferenceHub.transform.localScale = value;
                foreach (Controllers.Player target in Controllers.Player.List)
                    Network.SendSpawnMessage?.Invoke(null, [_player.ClassManager.netIdentity, target.Connection]);
            }
            catch (Exception ex)
            {
                Log.Error($"Scale error: {ex}");
            }
        }
    }
}