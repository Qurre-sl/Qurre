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
        get => Transform.rotation;
        set
        {
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

    public bool PreventScpDamage
    {
        get => Breakable._preventScpDamage;
        set => Breakable._preventScpDamage = value;
    }

    public float Hp
    {
        get => Breakable.health;
        set => Breakable.health = value;
    }

    public bool IsBroken
    {
        get => Breakable.NetworkisBroken;
        set => Breakable.NetworkisBroken = value;
    }
}