using Footprinting;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Controllers.Components;
using Qurre.API.World;
using UnityEngine;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Window : NetTransform
{
    private string _name;

    internal Window(BreakableWindow window)
    {
        _name = "Window";
        Breakable = window;
    }

    public BreakableWindow Breakable { get; }
    public bool AllowBreak { get; set; } = true;

    public override GameObject GameObject => Breakable.gameObject;
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

    public override void Destroy()
    {
        NetworkServer.Destroy(GameObject);
        Map.Windows.Remove(this);
    }
}