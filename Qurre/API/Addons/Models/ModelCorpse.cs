using System;
using JetBrains.Annotations;
using Mirror;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.Addons.Models;

[PublicAPI]
public class ModelCorpse
{
    public ModelCorpse(Model model, RoleTypeId type, Vector3 position, Vector3 rotation,
        Vector3 size = default, DamageHandlerBase? handler = null, string nickname = "")
    {
        handler ??= new CustomReasonDamageHandler("yes");

        Body = new Corpse(type, position, Quaternion.Euler(rotation), handler, nickname);
        GameObject = Body.GameObject;

        try
        {
            NetworkServer.UnSpawn(GameObject);
            GameObject.transform.parent = model.GameObject.transform;
            GameObject.transform.localPosition = position;
            GameObject.transform.localRotation = Quaternion.Euler(rotation);
            GameObject.transform.localScale = size;
            NetworkServer.Spawn(GameObject);

            RagdollData info = Body.Base.Info;
            Body.Base.NetworkInfo = new RagdollData(info.OwnerHub, info.Handler, GameObject.transform.position,
                GameObject.transform.rotation);
        }
        catch (Exception ex)
        {
            Log.Warn($"{ex}\n{ex.StackTrace}");
        }
    }

    public GameObject GameObject { get; }

    public Corpse Body { get; }
}