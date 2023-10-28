using Mirror;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using Qurre.API.Controllers;
using System;
using UnityEngine;

namespace Qurre.API.Addons.Models
{
    public class ModelBody
    {
        private readonly GameObject gameObject;
        private readonly Ragdoll body;

        public GameObject GameObject => gameObject;
        public Ragdoll Body => body;

        public ModelBody(Model model, RoleTypeId type, Vector3 position, Vector3 rotation,
            Vector3 size = default, DamageHandlerBase handler = null, string nickname = "")
        {
            try
            {
                if (handler is null) handler = new CustomReasonDamageHandler("yes");

                body = new(type, position, Quaternion.Euler(rotation), handler, nickname);
                gameObject = Body.GameObject;

                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.parent = model?.GameObject?.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;
                NetworkServer.Spawn(GameObject);

                var info = Body.ragdoll.Info;
                Body.ragdoll.NetworkInfo = new RagdollData(info.OwnerHub, info.Handler, GameObject.transform.position, GameObject.transform.rotation);
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }
    }
}