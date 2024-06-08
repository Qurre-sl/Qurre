using Qurre.API.Controllers;
using UnityEngine;

namespace Qurre.API.Addons.Models
{
    public class ModelPickup
    {
        private readonly GameObject gameObject;
        private readonly Pickup pickup;

        public GameObject GameObject => gameObject;
        public Pickup Pickup => pickup;

        public ModelPickup(Model model, ItemType type, Vector3 position, Vector3 rotation, Vector3 size = default, bool kinematic = true)
        {
            try
            {
                GameObject transformer = new("GetPosition");
                transformer.transform.parent = model.GameObject.transform;
                transformer.transform.localPosition = position;
                transformer.transform.localRotation = Quaternion.Euler(rotation);
                transformer.transform.localScale = size;

                pickup = new Item(type).Spawn(transformer.transform.position, transformer.transform.rotation, transformer.transform.lossyScale);
                gameObject = pickup.GameObject;

                if (kinematic)
                    gameObject.GetComponent<Rigidbody>().isKinematic = kinematic;

                Object.Destroy(transformer);
            }
            catch (System.Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }
    }
}