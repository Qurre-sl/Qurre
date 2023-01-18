using System;
using Hazards;
using Mirror;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Sinkhole
    {
        internal Sinkhole(SinkholeEnvironmentalHazard hole) => EnvironmentalHazard = hole;

        public SinkholeEnvironmentalHazard EnvironmentalHazard { get; }

        public GameObject GameObject => EnvironmentalHazard.gameObject;
        public Transform Transform => GameObject.transform;

        public string Name => GameObject.name;

        public Vector3 Position
        {
            get => Transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                Transform.position = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        public Quaternion Rotation
        {
            get => Transform.localRotation;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                Transform.localRotation = value;
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

        public bool ImmunityScps { get; set; }

        public float MaxDistance
        {
            get => EnvironmentalHazard.MaxDistance;
            set => EnvironmentalHazard.MaxDistance = value;
        }

        public float MaxHeightDistance
        {
            get => EnvironmentalHazard.MaxHeightDistance;
            set => EnvironmentalHazard.MaxHeightDistance = value;
        }

        public static bool operator ==(Sinkhole First, Sinkhole Next)
        {
            if (First is null && Next is null)
            {
                return true;
            }

            if (First is null && Next is not null)
            {
                return false;
            }

            if (First is not null && Next is null)
            {
                return false;
            }

            return First.GameObject == Next.GameObject;
        }

        public static bool operator !=(Sinkhole First, Sinkhole Next) => !(First == Next);

        public override bool Equals(object obj)
        {
            if (obj is Sinkhole)
            {
                return this == obj as Sinkhole;
            }

            var hole = obj as Sinkhole;

            if (obj is not null)
            {
                return this == hole;
            }

            return false;
        }

        public override int GetHashCode() => Tuple.Create(this, GameObject).GetHashCode();
    }
}