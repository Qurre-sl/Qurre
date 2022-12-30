using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Camera
    {
        internal readonly Scp079Camera _camera;

        public Scp079Camera Base => _camera;
        public GameObject GameObject => _camera.gameObject;

        public bool Active
        {
            get => _camera.IsActive;
            set => _camera.IsActive = value;
        }
        public bool Main => _camera.IsMain;

        public Room Room { get; private set; }

        internal Camera(Scp079Camera camera, Room room)
        {
            _camera = camera;
            Room = room;
        }
    }
}