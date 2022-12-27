using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    using Qurre.API.Addons;
    using UnityEngine;

    public class MovementState
    {
        private readonly Player _player;
        internal MovementState(Player pl)
        {
            _player = pl;
            
        }
        public Vector3 Postion
        {
            get => _player.GameObject.transform.position;
            set => _player.GameObject.transform.position = value;  
        }
        public Quaternion Rotation
        {
            get => _player.GameObject.transform.rotation;
            set => _player.GameObject.transform.rotation = value;
        }
        public Vector3 Scale
        {
            get => _player.GameObject.transform.localScale;
            set => _player.GameObject.transform.localScale = value;
        }
    }
}
