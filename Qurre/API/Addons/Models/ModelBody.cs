using System;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.Addons.Models
{
    public class ModelBody
    {
        public ModelBody(Model model, RoleTypeId type, Vector3 position, Vector3 rotation,
            Vector3 size = default, DamageHandlerBase handler = null, string nickname = "")
        {
            try
            {
                if (handler is null)
                {
                    handler = new CustomReasonDamageHandler("yes");
                }

                Body = new (type, position, Quaternion.Euler(rotation), handler, nickname);
                GameObject = Body.GameObject;

                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.parent = model?.GameObject?.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;
                NetworkServer.Spawn(GameObject);

                RagdollData info = Body.ragdoll.Info;
                Body.ragdoll.NetworkInfo = new (info.OwnerHub, info.Handler, GameObject.transform.position, GameObject.transform.rotation);
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }

        public GameObject GameObject { get; }

        public Ragdoll Body { get; }
    }
}