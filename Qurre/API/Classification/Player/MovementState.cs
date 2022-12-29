using PlayerRoles.FirstPersonControl;
using UnityEngine;
using System;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;

    public class MovementState
    {
        private readonly Player _player;
        internal MovementState(Player pl)
        {
            _player = pl;
        }

        public Transform CameraReference => _player.ReferenceHub.PlayerCameraReference;
        public Transform Transform => _player.ReferenceHub.transform;

        public Vector3 Position
        {
            get => _player.GameObject.transform.position;
            set => _player.ReferenceHub.TryOverridePosition(value, Vector3.zero);
        }
        public Vector3 Rotation
        {
            get => _player.GameObject.transform.eulerAngles;
            set => _player.ReferenceHub.TryOverridePosition(Position, value);
        }
        public Vector3 Scale
        {
            get => _player.ReferenceHub.transform.localScale;
            set
            {
                try
                {
                    _player.ReferenceHub.transform.localScale = value;
                    foreach (Player target in Player.List)
                        Network.SendSpawnMessage?.Invoke(null, new object[] { _player.ClassManager.netIdentity, target.Connection });
                }
                catch (Exception ex)
                {
                    Log.Error($"Scale error: {ex}");
                }
            }
        }
    }
}