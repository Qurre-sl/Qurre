using UnityEngine;
using System;
using Mirror;
using Object = UnityEngine.Object;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;

namespace Qurre.API.Addons.Models
{
    public class ModelPickup
    {
        private readonly GameObject gameObject;
        private readonly ItemPickupBase pickup;

        public GameObject GameObject => gameObject;
        public ItemPickupBase Pickup => pickup;

        public ModelPickup(Model model, ItemType type, Vector3 position, Vector3 rotation, Vector3 size = default, bool kinematic = true)
        {
            try
            {
                var item = Server.InventoryHost.CreateItemInstance(new(type, ItemSerialGenerator.GenerateNext()), true);
                ushort ser = ItemSerialGenerator.GenerateNext();

                Vector3 pos = model.GameObject.transform.position + position;
                Quaternion rot = Quaternion.Euler(model.GameObject.transform.rotation.eulerAngles + rotation);

                ItemPickupBase ipb = Object.Instantiate(item.PickupDropModel, pos, rot);

                ipb.Info.Serial = ser;
                ipb.Info.ItemId = type;
                ipb.Info.WeightKg = item.Weight;
                ipb.NetworkInfo = ipb.Info;

                ipb.Position = pos;
                ipb.Rotation = rot;

                gameObject = ipb.gameObject;
                GameObject.GetComponent<Rigidbody>().isKinematic = kinematic;
                GameObject.transform.parent = model.GameObject.transform;
                GameObject.transform.localPosition = position;
                GameObject.transform.localRotation = Quaternion.Euler(rotation);
                GameObject.transform.localScale = size;

                NetworkServer.Spawn(GameObject);

                pickup = ipb;
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex}\n{ex.StackTrace}");
            }
        }
    }
}