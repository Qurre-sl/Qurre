using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

namespace Qurre.API.Addons.Models;

/// <summary>
///     <para>Example:</para>
///     <para>var Model = new Model("Test", position, rotation);</para>
///     <para>
///         Model.AddPart(new ModelPrimitive(Model, PrimitiveType.Cube, new Color32(0, 0, 0, 155), Vector3.zero,
///         Vector3.zero, Vector3.one));
///     </para>
///     <para>Model.AddPart(new ModelLight(Model, new Color(1, 0, 0), Vector3.zero, 1, 10));</para>
/// </summary>
[PublicAPI]
public class Model
{
    private static readonly List<Model> ModelsList = [];
    private readonly List<ModelCorpse> _corpses = [];
    private readonly List<ModelDoor> _doors = [];

    private readonly List<ModelGenerator> _generators = [];
    private readonly List<ModelLight> _lights = [];
    private readonly List<ModelLocker> _lockers = [];
    private readonly Dictionary<GameObject, ModelEnums> _parts = [];
    private readonly List<ModelPickup> _pickups = [];
    private readonly List<ModelPrimitive> _primitives = [];
    private readonly List<ModelTarget> _targets = [];
    private readonly List<ModelWorkStation> _workStations = [];

    public Model(string id, Vector3 position, Vector3 rotation = default, Model? root = null)
        : this(id, position, rotation, Vector3.one, root)
    {
    }

    public Model(string id, Vector3 position, Vector3 rotation, Vector3 scale, Model? root = null)
    {
        GameObject = new GameObject(id)
        {
            transform =
            {
                parent = root?.GameObject.transform,
                localPosition = position,
                localRotation = Quaternion.Euler(rotation),
                localScale = scale == default ? Vector3.one : scale
            }
        };

        NetworkServer.Spawn(GameObject);

        ModelsList.Add(this);
    }

    public GameObject GameObject { get; }

    public IReadOnlyList<ModelCorpse> Corpses => _corpses.AsReadOnly();
    public IReadOnlyList<ModelDoor> Doors => _doors.AsReadOnly();
    public IReadOnlyList<ModelGenerator> Generators => _generators.AsReadOnly();
    public IReadOnlyList<ModelLight> Lights => _lights.AsReadOnly();
    public IReadOnlyList<ModelLocker> Lockers => _lockers.AsReadOnly();
    public IReadOnlyList<ModelPickup> Pickups => _pickups.AsReadOnly();
    public IReadOnlyList<ModelPrimitive> Primitives => _primitives.AsReadOnly();
    public IReadOnlyList<ModelTarget> Targets => _targets.AsReadOnly();
    public IReadOnlyList<ModelWorkStation> WorkStations => _workStations.AsReadOnly();

    public void AddPart(ModelCorpse corpse, bool addToList = true)
    {
        if (addToList)
            _corpses.Add(corpse);

        _parts.Add(corpse.GameObject, ModelEnums.Body);
    }

    public void AddPart(ModelDoor door, bool addToList = true)
    {
        if (addToList)
            _doors.Add(door);

        _parts.Add(door.GameObject, ModelEnums.Door);
    }

    public void AddPart(ModelGenerator gen, bool addToList = true)
    {
        if (addToList)
            _generators.Add(gen);

        _parts.Add(gen.GameObject, ModelEnums.Generator);
    }

    public void AddPart(ModelLight light, bool addToList = true)
    {
        if (addToList)
            _lights.Add(light);

        _parts.Add(light.GameObject, ModelEnums.Light);
    }

    public void AddPart(ModelLocker locker, bool addToList = true)
    {
        if (addToList)
            _lockers.Add(locker);

        _parts.Add(locker.GameObject, ModelEnums.Locker);
    }

    public void AddPart(ModelPickup pick, bool addToList = true)
    {
        if (addToList)
            _pickups.Add(pick);

        _parts.Add(pick.GameObject, ModelEnums.Pickup);
    }

    public void AddPart(ModelPrimitive prim, bool addToList = true)
    {
        if (addToList)
            _primitives.Add(prim);

        _parts.Add(prim.GameObject, ModelEnums.Primitive);
    }

    public void AddPart(ModelTarget shoot, bool addToList = true)
    {
        if (addToList)
            _targets.Add(shoot);

        _parts.Add(shoot.GameObject, ModelEnums.Target);
    }

    public void AddPart(ModelWorkStation station, bool addToList = true)
    {
        if (addToList)
            _workStations.Add(station);

        _parts.Add(station.GameObject, ModelEnums.WorkStation);
    }

    public void Destroy()
    {
        if (_parts.Count == 0)
            return;

        _parts.Select(x => x.Key).ForEach(NetworkServer.Destroy);

        Object.Destroy(GameObject);
        _parts.Clear();
        _corpses.Clear();
        _doors.Clear();
        _generators.Clear();
        _lights.Clear();
        _lockers.Clear();
        _pickups.Clear();
        _primitives.Clear();
        _targets.Clear();
        _workStations.Clear();
    }

    internal static void ClearCache()
    {
        ModelsList.ForEach(model =>
        {
            model._parts.Clear();
            model._corpses.Clear();
            model._doors.Clear();
            model._generators.Clear();
            model._lights.Clear();
            model._lockers.Clear();
            model._pickups.Clear();
            model._primitives.Clear();
            model._targets.Clear();
            model._workStations.Clear();
        });
        ModelsList.Clear();
    }
}