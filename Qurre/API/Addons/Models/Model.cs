using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace Qurre.API.Addons.Models
{
    /// <summary>
    ///  <para>Example:</para>
    ///  <para>var Model = new Model("Test", position, rotation);</para>
    ///  <para>Model.AddPart(new ModelPrimitive(Model, PrimitiveType.Cube, new Color32(0, 0, 0, 155), Vector3.zero, Vector3.zero, Vector3.one));</para>
    ///  <para>Model.AddPart(new ModelLight(Model, new Color(1, 0, 0), Vector3.zero, 1, 10));</para>
    /// </summary>
    public class Model
    {
        static readonly List<Model> ModelsList = new();
        internal readonly Dictionary<GameObject, ModelEnums> parts = new();

        readonly GameObject gameObject;
        readonly List<ModelBody> body = new();
        readonly List<object> bots = new(); // ModelBot
        readonly List<ModelDoor> doors = new();
        readonly List<ModelGenerator> generators = new();
        internal readonly List<ModelLight> lights = new();
        readonly List<ModelLocker> lockers = new();
        readonly List<ModelPickup> pickups = new();
        readonly List<ModelPrimitive> primitives = new();
        readonly List<ModelTarget> targets = new();
        readonly List<ModelWorkStation> workStations = new();

        public GameObject GameObject => gameObject;
        public IReadOnlyList<ModelBody> Body => body.AsReadOnly();
        public IReadOnlyList<object> Bots => bots.AsReadOnly(); // ModelBot
        public IReadOnlyList<ModelDoor> Doors => doors.AsReadOnly();
        public IReadOnlyList<ModelGenerator> Generators => generators.AsReadOnly();
        public IReadOnlyList<ModelLight> Lights => lights.AsReadOnly();
        public IReadOnlyList<ModelLocker> Lockers => lockers.AsReadOnly();
        public IReadOnlyList<ModelPickup> Pickups => pickups.AsReadOnly();
        public IReadOnlyList<ModelPrimitive> Primitives => primitives.AsReadOnly();
        public IReadOnlyList<ModelTarget> Targets => targets.AsReadOnly();
        public IReadOnlyList<ModelWorkStation> WorkStations => workStations.AsReadOnly();

        public void AddPart(ModelBody ragdoll, bool addToList = true)
        {
            if (addToList)
                body.Add(ragdoll);

            parts.Add(ragdoll.GameObject, ModelEnums.Body);
        }
        /*
        public void AddPart(ModelBot part, bool addToList = true)
        {
            if (addToList) bots.Add(part);
            parts.Add(part.GameObject, ModelEnums.Bot);
        }
        */
        public void AddPart(ModelDoor door, bool addToList = true)
        {
            if (addToList)
                doors.Add(door);

            parts.Add(door.GameObject, ModelEnums.Door);
        }
        public void AddPart(ModelGenerator gen, bool addToList = true)
        {
            if (addToList)
                generators.Add(gen);

            parts.Add(gen.GameObject, ModelEnums.Generator);
        }
        public void AddPart(ModelLight light, bool addToList = true)
        {
            if (addToList)
                lights.Add(light);

            parts.Add(light.GameObject, ModelEnums.Light);
        }
        public void AddPart(ModelLocker locker, bool addToList = true)
        {
            if (addToList)
                lockers.Add(locker);

            parts.Add(locker.GameObject, ModelEnums.Locker);
        }
        public void AddPart(ModelPickup pick, bool addToList = true)
        {
            if (addToList)
                pickups.Add(pick);

            parts.Add(pick.GameObject, ModelEnums.Pickup);
        }
        public void AddPart(ModelPrimitive prim, bool addToList = true)
        {
            if (addToList)
                primitives.Add(prim);

            parts.Add(prim.GameObject, ModelEnums.Primitive);
        }
        public void AddPart(ModelTarget shoot, bool addToList = true)
        {
            if (addToList)
                targets.Add(shoot);

            parts.Add(shoot.GameObject, ModelEnums.Target);
        }
        public void AddPart(ModelWorkStation station, bool addToList = true)
        {
            if (addToList)
                workStations.Add(station);

            parts.Add(station.GameObject, ModelEnums.WorkStation);
        }

        public Model(string id, Vector3 position, Vector3 rotation = default, Model root = null)
            : this(id, position, rotation, Vector3.one, root) { }

        public Model(string id, Vector3 position, Vector3 rotation, Vector3 scale, Model root = null)
        {
            gameObject = new GameObject(id);
            GameObject.transform.parent = root?.GameObject?.transform;
            GameObject.transform.localPosition = position;
            GameObject.transform.localRotation = Quaternion.Euler(rotation);
            GameObject.transform.localScale = scale == default ? Vector3.one : scale;

            NetworkServer.Spawn(GameObject);

            ModelsList.Add(this);
        }

        public void Destroy()
        {
            if (parts.Count == 0)
                return;

            var _list = parts.Select(x => x.Key).ToList();
            _list.ForEach(part =>
            {
                NetworkServer.Destroy(part);
            });

            Object.Destroy(GameObject);
            parts.Clear();
            body.Clear();
            bots.Clear();
            doors.Clear();
            generators.Clear();
            lights.Clear();
            lockers.Clear();
            pickups.Clear();
            primitives.Clear();
            targets.Clear();
            workStations.Clear();
        }

        static internal void ClearCache()
        {
            ModelsList.ForEach(model =>
            {
                try
                {
                    model.parts.Clear();
                    model.body.Clear();
                    model.bots.Clear();
                    model.doors.Clear();
                    model.generators.Clear();
                    model.lights.Clear();
                    model.lockers.Clear();
                    model.pickups.Clear();
                    model.primitives.Clear();
                    model.targets.Clear();
                    model.workStations.Clear();
                }
                catch { }
            });
            ModelsList.Clear();
        }
    }
}