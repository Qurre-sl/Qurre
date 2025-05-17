using JetBrains.Annotations;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Camera
{
    internal Camera(Scp079Camera camera, Room room)
    {
        Base = camera;
        Room = room;
    }

    public Scp079Camera Base { get; }
    public Room Room { get; }

    public GameObject GameObject => Base.gameObject;
    public bool Main => Base.IsMain;

    public bool Active
    {
        get => Base.IsActive;
        set => Base.IsActive = value;
    }
}