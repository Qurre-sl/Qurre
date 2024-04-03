using Mirror;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Ragdoll
    {
        private Player _pl;
        internal readonly BasicRagdoll ragdoll;

        public GameObject GameObject => ragdoll.gameObject;
        public string Name => ragdoll.name;

        public Vector3 Position
        {
            get
            {
                try { return ragdoll.transform.position; }
                catch { return Vector3.zero; }
            }
            set
            {
                NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.position = value;
                NetworkServer.Spawn(GameObject);

                var info = ragdoll.Info;
                ragdoll.NetworkInfo = new RagdollData(info.OwnerHub, info.Handler, value, info.StartRotation);
            }
        }
        public Quaternion Rotation
        {
            get => ragdoll.transform.localRotation;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.localRotation = value;
                NetworkServer.Spawn(GameObject);

                var info = ragdoll.Info;
                ragdoll.NetworkInfo = new RagdollData(info.OwnerHub, info.Handler, info.StartPosition, value);
            }
        }
        public Vector3 Scale
        {
            get => ragdoll.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.localScale = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        public Player Owner
        {
            get => _pl;
            set
            {
                _pl = value;
                var info = ragdoll.Info;
                ragdoll.NetworkInfo = new RagdollData(value.ReferenceHub, info.Handler, info.StartPosition, info.StartRotation);
            }
        }

        public void Destroy()
        {
            NetworkServer.Destroy(GameObject);
            Map.Ragdolls.Remove(this);
        }

        internal Ragdoll(BasicRagdoll @base, Player owner)
        {
            ragdoll = @base;
            _pl = owner;
        }
        public Ragdoll(RoleTypeId type, Vector3 position, Quaternion rotation, DamageHandlerBase handler, Player owner)
            : this(type, position, rotation, handler, owner.UserInfomation.Nickname) => _pl = Server.Host;
        public Ragdoll(Vector3 position, Quaternion rotation, DamageHandlerBase handler, Player owner)
            : this(owner.RoleInfomation.Role, position, rotation, handler, owner) { }
        public Ragdoll(RoleTypeId type, Vector3 position, Quaternion rotation, DamageHandlerBase handler, string nickname)
        {
            if (!PlayerRoleLoader.AllRoles.TryFind(out var role, x => x.Key == type))
                throw new System.Exception("Role not found: " + type);

            if (role.Value is not IRagdollRole irag)
                return;

            GameObject gameObject = Object.Instantiate(irag.Ragdoll.gameObject);

            if (!gameObject.TryGetComponent(out BasicRagdoll component))
                return;

            ragdoll = component;
            _pl = Server.Host;

            ragdoll.NetworkInfo = new RagdollData(Server.Host.ReferenceHub, handler,
                type, position, rotation, nickname, NetworkTime.time);

            NetworkServer.Spawn(component.gameObject);

            Map.Ragdolls.Add(this);

            try
            {
                if (Owner != null)
                {
                    var s1 = Scale;
                    var s2 = Owner.MovementState.Scale;
                    Scale = new Vector3(s1.x * s2.x, s1.y * s2.y, s1.z * s2.z);
                }
            }
            catch { }
        }
    }
}