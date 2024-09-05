using Interactables.Interobjects;
using InventorySystem.Items.Firearms.Attachments;
using MapGeneration.Distributors;
using Mirror;
using Qurre.API.Objects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Qurre.API.Addons
{
    static public class Prefabs
    {
        static readonly Dictionary<DoorPrefabs, BreakableDoor> _doors = new();
        static readonly Dictionary<LockerPrefabs, Locker> _lockers = new();
        static readonly Dictionary<TargetPrefabs, GameObject> _targets = new();
        static WorkstationController _work;
        static Scp079Generator _generator;
        static GameObject _primitive;
        static GameObject _light;
        static GameObject _tantrum;
        static GameObject _cloud;

        static public IReadOnlyDictionary<DoorPrefabs, BreakableDoor> Doors => _doors;
        static public IReadOnlyDictionary<LockerPrefabs, Locker> Lockers => _lockers;
        static public IReadOnlyDictionary<TargetPrefabs, GameObject> Targets => _targets;

        static public WorkstationController WorkStation => _work;
        static public Scp079Generator Generator => _generator;
        static public GameObject Primitive => _primitive;
        static public GameObject Light => _light;
        static public GameObject Tantrum => _tantrum;
        static public GameObject Cloud => _cloud;


        static internal void Init()
        {
            try
            {
                foreach (var prefab in NetworkManager.singleton.spawnPrefabs)
                {
                    //Log.Custom(prefab.name, "Init", ConsoleColor.Magenta);
                    switch (prefab.name)
                    {
                        case "EZ BreakableDoor" when prefab.TryGetComponent<BreakableDoor>(out var door):
                            _doors[DoorPrefabs.DoorEZ] = door;
                            break;
                        case "HCZ BreakableDoor" when prefab.TryGetComponent<BreakableDoor>(out var door):
                            _doors[DoorPrefabs.DoorHCZ] = door;
                            break;
                        case "LCZ BreakableDoor" when prefab.TryGetComponent<BreakableDoor>(out var door):
                            _doors[DoorPrefabs.DoorLCZ] = door;
                            break;

                        case "TantrumObj":
                            {
                                _tantrum = prefab;
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error in Addons => Prefabs [Init]:\n{e}\n{e.StackTrace}");
            }
        }

        static internal void InitLate()
        {
            try
            {
                foreach (var prefab in NetworkClient.prefabs)
                {
                    //Log.Custom(prefab.Key + " - " + prefab.Value.name, "InitLate", ConsoleColor.Blue);
                    switch (prefab.Key.ToString())
                    {
                        case "2724603877" when prefab.Value.TryGetComponent<Scp079Generator>(out var gen):
                            _generator = gen;
                            break;

                        case "1783091262" when prefab.Value.TryGetComponent<WorkstationController>(out var station):
                            _work = station;
                            break;

                        case "2286635216" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.Pedestal018] = locker;
                            break;
                        case "664776131" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.Pedestal207] = locker;
                            break;
                        case "3724306703" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.Pedestal244] = locker;
                            break;
                        case "3849573771" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.Pedestal268] = locker;
                            break;
                        case "373821065" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.Pedestal500] = locker;
                            break;
                        case "3372339835" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.Pedestal1576] = locker;
                            break;
                        case "3962534659" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.Pedestal1853] = locker;
                            break;
                        case "3578915554" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.Pedestal2176] = locker;
                            break;

                        case "2830750618" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.LargeGun] = locker;
                            break;
                        case "3352879624" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.RifleRack] = locker;
                            break;
                        case "1964083310" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.MiscLocker] = locker;
                            break;

                        case "4040822781" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.RegularMedkit] = locker;
                            break;
                        case "2525847434" when prefab.Value.TryGetComponent<Locker>(out var locker):
                            _lockers[LockerPrefabs.AdrenalineMedkit] = locker;
                            break;

                        case "1704345398":
                            _targets[TargetPrefabs.Sport] = prefab.Value;
                            break;
                        case "858699872":
                            _targets[TargetPrefabs.Dboy] = prefab.Value;
                            break;
                        case "3613149668":
                            _targets[TargetPrefabs.Binary] = prefab.Value;
                            break;

                        case "1321952889":
                            _primitive = prefab.Value;
                            break;
                        case "3956448839":
                            _light = prefab.Value;
                            break;
                        case "825024811":
                            _cloud = prefab.Value;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error in Addons => Prefabs [InitLate]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}