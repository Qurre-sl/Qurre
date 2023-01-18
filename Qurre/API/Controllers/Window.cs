using Footprinting;
using Mirror;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Window
    {
        private string name;

        internal Window(BreakableWindow window) => Breakable = window;

        public BreakableWindow Breakable { get; }

        public GameObject GameObject => Breakable.gameObject;
        public Transform Transform => Breakable.transform;

        public bool AllowBreak { get; set; } = true;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = "Window";
                }

                return name;
            }
            set => name = value;
        }

        public Vector3 Position
        {
            get => Status.position;
            set
            {
                BreakableWindow.BreakableWindowStatus _status = Status;
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
                BreakableWindow.BreakableWindowStatus _status = Status;
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

        public Vector3 Size => Breakable.size;
        public Footprint LastAttacker => Breakable.LastAttacker;
        public bool Destroyed => Breakable.isBroken;

        public float Hp
        {
            get => Breakable.health;
            set => Breakable.health = value;
        }

        public BreakableWindow.BreakableWindowStatus Status
        {
            get => Breakable.NetworksyncStatus;
            set => Breakable.UpdateStatus(value);
        }
    }
}