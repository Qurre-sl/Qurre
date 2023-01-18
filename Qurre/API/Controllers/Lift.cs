using Interactables.Interobjects;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Lift
    {
        internal Lift(ElevatorChamber elevator) => Elevator = elevator;

        public ElevatorChamber Elevator { get; }

        public GameObject GameObject => Elevator.gameObject;
        public Transform Transform => Elevator.transform;

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

        public Vector3 Scale => Transform.localScale;

        public ElevatorChamber.ElevatorSequence Status
        {
            get => Elevator._curSequence;
            set => Elevator._curSequence = value;
        }

        public ElevatorManager.ElevatorGroup Type => Elevator.AssignedGroup;

        public void Use() => Status = ElevatorChamber.ElevatorSequence.Ready;
    }
}