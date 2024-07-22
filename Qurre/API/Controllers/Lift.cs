using Interactables.Interobjects;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Lift
    {
        public ElevatorChamber Elevator
            => _elevator;

        public GameObject GameObject
            => _elevator.gameObject;

        public Transform Transform
            => _elevator.transform;

        public Vector3 Position
        {
            get => Transform.position;
            set => Transform.position = value;
        }
        public Quaternion Rotation
        {
            get => Transform.rotation;
            set => Transform.rotation = value;
        }
        public Vector3 Scale
            => Transform.localScale;

        public ElevatorChamber.ElevatorSequence Status
        {
            get => _elevator._curSequence;
            set => _elevator._curSequence = value;
        }

        public ElevatorManager.ElevatorGroup Type
            => _elevator.AssignedGroup;

        public Bounds Bounds
            => _elevator.WorldspaceBounds;

        public void Use() => Status = ElevatorChamber.ElevatorSequence.Ready;


        private readonly ElevatorChamber _elevator;
        internal Lift(ElevatorChamber elevator) => _elevator = elevator;
    }
}