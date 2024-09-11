using Footprinting;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Window
{
    private string _name;

    internal Window(BreakableWindow window)
    {
        _name = "Window";
        Breakable = window;
    }

    public BreakableWindow Breakable { get; }
    public bool AllowBreak { get; set; } = true;

    public GameObject GameObject => Breakable.gameObject;
    public Transform Transform => Breakable.transform;
    public Footprint LastAttacker => Breakable.LastAttacker;
    public bool Destroyed => Breakable.isBroken;
    public Vector3 Size => Breakable.size;

    public string Name
    {
        get
        {
            if (string.IsNullOrEmpty(_name))
                _name = "Window";

            return _name;
        }
        set => _name = value;
    }

    public Vector3 Position
    {
        get => Status.position;
        set
        {
            BreakableWindow.BreakableWindowStatus status = Status;
            status.position = value;
            Status = status;

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
            BreakableWindow.BreakableWindowStatus status = Status;
            status.rotation = value;
            Status = status;

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