using System;
using System.Collections.Generic;
using Interactables.Interobjects;
using InventorySystem.Items.Firearms.Attachments;
using MapGeneration.Distributors;
using Mirror;
using Qurre.API.Objects;
using UnityEngine;

namespace Qurre.API.Addons
{
    public static class Prefabs
    {
        private static readonly Dictionary<DoorPrefabs, BreakableDoor> _doors = new ();

        private static readonly Dictionary<LockerPrefabs, Locker> _lockers = new ();

        private static readonly Dictionary<TargetPrefabs, GameObject> _targets = new ();

        public static IReadOnlyDictionary<DoorPrefabs, BreakableDoor> Doors => _doors;

        public static IReadOnlyDictionary<LockerPrefabs, Locker> Lockers => _lockers;

        public static IReadOnlyDictionary<TargetPrefabs, GameObject> Targets => _targets;

        public static WorkstationController WorkStation { get; private set; }

        public static Scp079Generator Generator { get; private set; }

        public static GameObject Primitive { get; private set; }

        public static GameObject Light { get; private set; }

        internal static void Init()
        {
            try
            {
                foreach (GameObject prefab in NetworkManager.singleton.spawnPrefabs)
                {
                    //Log.Custom(prefab.name, "Init", ConsoleColor.Magenta);
                    switch (prefab.name)
                    {
                        case "EZ BreakableDoor" when prefab.TryGetComponent(out BreakableDoor door):
                            _doors[DoorPrefabs.DoorEZ] = door;
                            break;
                        case "HCZ BreakableDoor" when prefab.TryGetComponent(out BreakableDoor door):
                            _doors[DoorPrefabs.DoorHCZ] = door;
                            break;
                        case "LCZ BreakableDoor" when prefab.TryGetComponent(out BreakableDoor door):
                            _doors[DoorPrefabs.DoorLCZ] = door;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error in Addons => Prefabs [Init]:\n{e}\n{e.StackTrace}");
            }
        }

        internal static void InitLate()
        {
            try
            {
                foreach (KeyValuePair<Guid, GameObject> prefab in NetworkClient.prefabs)
                {
                    //Log.Custom(prefab.Key + " - " + prefab.Value.name, "InitLate", ConsoleColor.Blue);
                    switch (prefab.Key.ToString())
                    {
                        case "daf3ccde-4392-c0e4-882d-b7002185c6b8" when prefab.Value.TryGetComponent(out Scp079Generator gen):
                            Generator = gen;
                            break;

                        case "ad8a455f-062d-dea4-5b47-ac9217d4c58b" when prefab.Value.TryGetComponent(out WorkstationController station):
                            WorkStation = station;
                            break;

                        case "a149d3eb-11bd-de24-f9dd-57187f5771ef" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.Pedestal018] = locker;
                            break;
                        case "17054030-9461-d104-5b92-9456c9eb0ab7" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.Pedestal207] = locker;
                            break;
                        case "fa602fdc-724c-d2a4-8b8c-1fb314b82746" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.Pedestal244] = locker;
                            break;
                        case "68f13209-e652-6024-2b89-0f75fb88a998" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.Pedestal268] = locker;
                            break;
                        case "f4149b66-c503-87a4-0b93-aabfe7c352da" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.Pedestal500] = locker;
                            break;
                        case "41b2f19d-5789-04b4-e910-bb068664bc8a" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.Pedestal1576] = locker;
                            break;
                        case "4f36c701-ea0c-9064-2a58-2c89240e51ba" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.Pedestal1853] = locker;
                            break;
                        case "fff1c10c-a719-bea4-d95c-3e262ed03ab2" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.Pedestal2176] = locker;
                            break;

                        case "5ad5dc6d-7bc5-3154-8b1a-3598b96e0d5b" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.LargeGun] = locker;
                            break;
                        case "850f84ad-e273-1824-8885-11ae5e01e2f4" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.RifleRack] = locker;
                            break;
                        case "d54bead1-286f-3004-facd-74482a872ad8" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.MiscLocker] = locker;
                            break;

                        case "5b227bd2-1ed2-8fc4-2aa1-4856d7cb7472" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.RegularMedkit] = locker;
                            break;
                        case "db602577-8d4f-97b4-890b-8c893bfcd553" when prefab.Value.TryGetComponent(out Locker locker):
                            _lockers[LockerPrefabs.AdrenalineMedkit] = locker;
                            break;

                        case "3353122b-0ba2-5d14-fa64-886c45425967":
                            _targets[TargetPrefabs.Sport] = prefab.Value;
                            break;
                        case "422b08ed-0bc0-6cb4-7a7f-81dd37c430c0":
                            _targets[TargetPrefabs.Dboy] = prefab.Value;
                            break;
                        case "4f03f7fa-f417-ae84-382b-962c31614d1a":
                            _targets[TargetPrefabs.Binary] = prefab.Value;
                            break;

                        case "bf9a7ae6-aaea-0174-d807-e0d4adb1c524":
                            Primitive = prefab.Value;
                            break;
                        case "6996edbf-2adf-a5b4-e8ce-e089cf9710ae":
                            Light = prefab.Value;
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