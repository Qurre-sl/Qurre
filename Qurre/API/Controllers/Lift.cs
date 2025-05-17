using System;
using Interactables.Interobjects;
using JetBrains.Annotations;
using UnityEngine;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Lift
{
    internal Lift(ElevatorChamber elevator)
    {
        Elevator = elevator;
    }

    public ElevatorChamber Elevator { get; }

    public GameObject GameObject
        => Elevator.gameObject;

    public Transform Transform
        => Elevator.transform;

    public ElevatorGroup Type
        => Elevator.AssignedGroup;

    public Bounds Bounds
        => Elevator.WorldspaceBounds;

    public Vector3 Scale
        => Transform.localScale;

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

    public ElevatorChamber.ElevatorSequence Status
    {
        get => Elevator.CurSequence;
        set => Elevator.CurSequence = value;
    }

    public void Use()
    {
        Status = ElevatorChamber.ElevatorSequence.Ready;
    }

    public static bool operator ==(Lift? lhs, Lift? rhs)
    {
        if (rhs is null)
            return lhs?.Transform == null;

        if (lhs is null)
            return rhs.Transform == null;

        return lhs.Transform == rhs.Transform;
    }

    public static bool operator !=(Lift lhs, Lift rhs)
    {
        return !(lhs == rhs);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Lift lift)
            return this == lift;

        return false;
    }

    public override int GetHashCode()
    {
        return Tuple.Create(this, GameObject).GetHashCode();
    }
}