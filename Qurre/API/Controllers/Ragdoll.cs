﻿using System.Collections.Generic;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;

namespace Qurre.API.Controllers
{
    public class Ragdoll
    {
        internal readonly BasicRagdoll ragdoll;
        private Player _pl;

        internal Ragdoll(BasicRagdoll @base, Player owner)
        {
            ragdoll = @base;
            _pl = owner;
        }

        public Ragdoll(RoleTypeId type, Vector3 position, Quaternion rotation, DamageHandlerBase handler, Player owner)
            : this(owner.RoleInfomation.Role, position, rotation, handler, owner.UserInfomation.Nickname) => _pl = Server.Host;

        public Ragdoll(Vector3 position, Quaternion rotation, DamageHandlerBase handler, Player owner)
            : this(owner.RoleInfomation.Role, position, rotation, handler, owner) { }

        public Ragdoll(RoleTypeId type, Vector3 position, Quaternion rotation, DamageHandlerBase handler, string nickname)
        {
            if (!PlayerRoleLoader.AllRoles.TryFind(out KeyValuePair<RoleTypeId, PlayerRoleBase> role, x => x.Key == type))
            {
                throw new ("Role not found: " + type);
            }

            if (role.Value is not IRagdollRole irag)
            {
                return;
            }

            GameObject gameObject = Object.Instantiate(irag.Ragdoll.gameObject);

            if (!gameObject.TryGetComponent(out BasicRagdoll component))
            {
                return;
            }

            ragdoll = component;
            _pl = Server.Host;

            ragdoll.NetworkInfo = new (
                Server.Host.ReferenceHub, handler,
                type, position, rotation, nickname, NetworkTime.time);

            NetworkServer.Spawn(component.gameObject);

            Map.Ragdolls.Add(this);

            try
            {
                if (Owner != null)
                {
                    Vector3 s1 = Scale;
                    Vector3 s2 = Owner.MovementState.Scale;
                    Scale = new (s1.x * s2.x, s1.y * s2.y, s1.z * s2.z);
                }
            }
            catch { }
        }

        public GameObject GameObject => ragdoll.gameObject;
        public string Name => ragdoll.name;

        public Vector3 Position
        {
            get
            {
                try
                {
                    return ragdoll.transform.position;
                }
                catch
                {
                    return Vector3.zero;
                }
            }
            set
            {
                NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.position = value;
                NetworkServer.Spawn(GameObject);

                RagdollData info = ragdoll.Info;
                ragdoll.NetworkInfo = new (info.OwnerHub, info.Handler, value, info.StartRotation);
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

                RagdollData info = ragdoll.Info;
                ragdoll.NetworkInfo = new (info.OwnerHub, info.Handler, info.StartPosition, value);
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
                RagdollData info = ragdoll.Info;
                ragdoll.NetworkInfo = new (value.ReferenceHub, info.Handler, info.StartPosition, info.StartRotation);
            }
        }

        public void Destroy()
        {
            Object.Destroy(GameObject);
            Map.Ragdolls.Remove(this);
        }
    }
}