using UnityEngine;
using Mirror;
using System;
using Hazards;

namespace Qurre.API.Controllers
{
    public class Sinkhole
    {
        private readonly SinkholeEnvironmentalHazard sinkhole;

        public SinkholeEnvironmentalHazard EnvironmentalHazard => sinkhole;
        public GameObject GameObject => sinkhole.gameObject;
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
            get => sinkhole.MaxDistance;
            set => sinkhole.MaxDistance = value;
        }
        public float MaxHeightDistance
        {
            get => sinkhole.MaxHeightDistance;
            set => sinkhole.MaxHeightDistance = value;
        }

        static public bool operator ==(Sinkhole First, Sinkhole Next)
        {
            if (First is null && Next is null) return true;
            else if (First is null && Next is not null) return false;
            else if (First is not null && Next is null) return false;
            else return First.GameObject == Next.GameObject;
        }
        static public bool operator !=(Sinkhole First, Sinkhole Next) => !(First == Next);
        public override bool Equals(object obj)
        {
            if (obj is Sinkhole)
            {
                return this == obj as Sinkhole;
            }
            else
            {
                Sinkhole hole = obj as Sinkhole;
                if (obj is not null) return this == hole;
                else return false;
            }
        }
        public override int GetHashCode() => Tuple.Create(this, GameObject).GetHashCode();

        public Sinkhole(SinkholeEnvironmentalHazard hole) => sinkhole = hole;
    }
}