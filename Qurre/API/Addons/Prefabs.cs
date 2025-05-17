using System;
using System.Collections.Generic;
using Interactables.Interobjects;
using InventorySystem.Items.Firearms.Attachments;
using JetBrains.Annotations;
using MapGeneration.Distributors;
using Mirror;
using Qurre.API.Objects;
using UnityEngine;

namespace Qurre.API.Addons;

[PublicAPI]
public static class Prefabs
{
    private static readonly Dictionary<DoorPrefabs, BreakableDoor> LocalDoors = [];
    private static readonly Dictionary<LockerPrefabs, Locker> LocalLockers = [];
    private static readonly Dictionary<TargetPrefabs, GameObject> LocalTargets = [];

    public static IReadOnlyDictionary<DoorPrefabs, BreakableDoor> Doors => LocalDoors;
    public static IReadOnlyDictionary<LockerPrefabs, Locker> Lockers => LocalLockers;
    public static IReadOnlyDictionary<TargetPrefabs, GameObject> Targets => LocalTargets;

    public static WorkstationController? WorkStation { get; private set; }

    public static Scp079Generator? Generator { get; private set; }

    public static GameObject? Primitive { get; private set; }

    public static GameObject? Light { get; private set; }


    public static GameObject? Speaker { get; private set; }

    public static GameObject? Tantrum { get; private set; }

    public static GameObject? Cloud { get; private set; }


    internal static void Init()
    {
        try
        {
            foreach (GameObject? prefab in NetworkManager.singleton.spawnPrefabs)
            {
#if TESTS
                Log.Custom(prefab.name, "Init", ConsoleColor.Magenta);
#endif

                switch (prefab.name)
                {
                    case "EZ BreakableDoor" when prefab.TryGetComponent<BreakableDoor>(out BreakableDoor? door):
                        LocalDoors[DoorPrefabs.DoorEZ] = door;
                        break;
                    case "HCZ BreakableDoor" when prefab.TryGetComponent<BreakableDoor>(out BreakableDoor? door):
                        LocalDoors[DoorPrefabs.DoorHCZ] = door;
                        break;
                    case "LCZ BreakableDoor" when prefab.TryGetComponent<BreakableDoor>(out BreakableDoor? door):
                        LocalDoors[DoorPrefabs.DoorLCZ] = door;
                        break;

                    case "TantrumObj":
                        {
                            Tantrum = prefab;
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

    internal static void InitLate()
    {
        try
        {
            foreach (var prefab in NetworkClient.prefabs)
            {
#if TESTS
                Log.Custom(prefab.Key + " - " + prefab.Value.name, "InitLate", ConsoleColor.Blue);
#endif

                switch (prefab.Key.ToString())
                {
                    case "2724603877" when prefab.Value.TryGetComponent<Scp079Generator>(out Scp079Generator? gen):
                        Generator = gen;
                        break;

                    case "1783091262"
                        when prefab.Value.TryGetComponent<WorkstationController>(out WorkstationController? station):
                        WorkStation = station;
                        break;

                    case "2286635216" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.Pedestal018] = locker;
                        break;
                    case "664776131" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.Pedestal207] = locker;
                        break;
                    case "3724306703" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.Pedestal244] = locker;
                        break;
                    case "3849573771" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.Pedestal268] = locker;
                        break;
                    case "373821065" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.Pedestal500] = locker;
                        break;
                    case "3372339835" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.Pedestal1576] = locker;
                        break;
                    case "3962534659" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.Pedestal1853] = locker;
                        break;
                    case "3578915554" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.Pedestal2176] = locker;
                        break;

                    case "2830750618" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.LargeGun] = locker;
                        break;
                    case "3352879624" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.RifleRack] = locker;
                        break;
                    case "1964083310" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.MiscLocker] = locker;
                        break;
                    case "2372810204" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.Experimental] = locker;
                        break;

                    case "4040822781" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.RegularMedkit] = locker;
                        break;
                    case "2525847434" when prefab.Value.TryGetComponent<Locker>(out Locker? locker):
                        LocalLockers[LockerPrefabs.AdrenalineMedkit] = locker;
                        break;

                    case "3038351124" when prefab.Value.TryGetComponent<BreakableDoor>(out BreakableDoor? door):
                        LocalDoors[DoorPrefabs.DoorLCZ] = door;
                        break;
                    case "2295511789" when prefab.Value.TryGetComponent<BreakableDoor>(out BreakableDoor? door):
                        LocalDoors[DoorPrefabs.DoorHCZ] = door;
                        break;
                    case "1883254029" when prefab.Value.TryGetComponent<BreakableDoor>(out BreakableDoor? door):
                        LocalDoors[DoorPrefabs.DoorEZ] = door;
                        break;
                    case "2176035362" when prefab.Value.TryGetComponent<BreakableDoor>(out BreakableDoor? door):
                        LocalDoors[DoorPrefabs.BulkHCZ] = door;
                        break;

                    case "1704345398":
                        LocalTargets[TargetPrefabs.Sport] = prefab.Value;
                        break;
                    case "858699872":
                        LocalTargets[TargetPrefabs.Dboy] = prefab.Value;
                        break;
                    case "3613149668":
                        LocalTargets[TargetPrefabs.Binary] = prefab.Value;
                        break;

                    case "1321952889":
                        Primitive = prefab.Value;
                        break;
                    case "3956448839":
                        Light = prefab.Value;
                        break;
                    case "825024811":
                        Cloud = prefab.Value;
                        break;
                    case "712426663":
                        Speaker = prefab.Value;
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