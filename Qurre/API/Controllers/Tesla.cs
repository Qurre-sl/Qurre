using System.Collections.Generic;
using Mirror;
using PlayerRoles;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Tesla
    {
        private string name;

        internal Tesla(TeslaGate gate) => Gate = gate;

        public TeslaGate Gate { get; }

        public GameObject GameObject => Gate.gameObject;
        public Transform Transform => GameObject.transform;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    return GameObject.name;
                }

                return name;
            }
            set => name = value;
        }

        public Vector3 Position => Transform.position;
        public Quaternion Rotation => Transform.localRotation;

        public Vector3 Scale
        {
            get => Transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                Transform.localScale = value;
                SizeOfKiller = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        public Vector3 SizeOfKiller
        {
            get => Gate.sizeOfKiller;
            set => Gate.sizeOfKiller = value;
        }

        public bool InProgress
        {
            get => Gate.InProgress;
            set => Gate.InProgress = value;
        }

        public float SizeOfTrigger
        {
            get => Gate.sizeOfTrigger;
            set => Gate.sizeOfTrigger = value;
        }

        public bool Enable { get; set; } = true;
        public bool Allow079Interact { get; set; } = true;

        public List<RoleTypeId> ImmunityRoles { get; } = new ();
        public List<Player> ImmunityPlayers { get; } = new ();

        public void Trigger(bool instant = false)
        {
            if (instant)
            {
                Gate.RpcInstantBurst();
            }
            else
            {
                Gate.RpcPlayAnimation();
            }
        }

        public void Destroy() => Object.Destroy(Gate.gameObject);
    }
}