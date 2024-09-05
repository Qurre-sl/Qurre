using Footprinting;
using Mirror;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Window
    {
        private readonly BreakableWindow bw;
        private string name;

        public BreakableWindow Breakable => bw;
        public GameObject GameObject => bw.gameObject;
        public Transform Transform => bw.transform;

        public bool AllowBreak { get; set; } = true;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name)) name = "Window";
                return name;
            }
            set => name = value;
        }

        public Vector3 Position
        {
            get => Status.position;
            set
            {
                var _status = Status;
                _status.position = value;
                Status = _status;

                NetworkServer.UnSpawn(GameObject);
                Transform.position = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public Quaternion Rotation
        {
            get => Status.rotation;
            set
            {
                var _status = Status;
                _status.rotation = value;
                Status = _status;

                NetworkServer.UnSpawn(GameObject);
                Transform.rotation = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public Vector3 Scale
        {
            get => Transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                Transform.localScale = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        public Vector3 Size => bw.size;
        public Footprint LastAttacker => bw.LastAttacker;
        public bool Destroyed => bw.isBroken;
        public float Hp
        {
            get => bw.health;
            set => bw.health = value;
        }
        public BreakableWindow.BreakableWindowStatus Status
        {
            get => bw.NetworksyncStatus;
            set => bw.UpdateStatus(value);
        }

        internal Window(BreakableWindow window) => bw = window;
    }
}