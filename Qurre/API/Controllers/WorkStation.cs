using System;
using InventorySystem.Items.Firearms.Attachments;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Addons;
using Qurre.API.Objects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class WorkStation
{
    internal WorkStation(WorkstationController station)
    {
        Controller = station;
    }

    public WorkStation(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        if (Prefabs.WorkStation == null)
            throw new NullReferenceException(nameof(Prefabs.WorkStation));

        Custom = true;
        
        Controller = Object.Instantiate(Prefabs.WorkStation, position, Quaternion.Euler(rotation));

        Controller.gameObject.transform.localScale = scale;
        NetworkServer.Spawn(Controller.gameObject);

        Map.WorkStations.Add(this);
    }

    public WorkstationController Controller { get; }
    
    public bool Custom { get; }

    public GameObject GameObject => Controller.gameObject;
    public Transform Transform => GameObject.transform;
    public string Name => GameObject.name;

    public Vector3 Position
    {
        get => Transform.position;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            Transform.position = value;
            NetworkServer.Spawn(GameObject);
        }
    }

    public Vector3 Scale
    {
        get => Transform.localScale;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            Transform.localScale = value;
            NetworkServer.Spawn(GameObject);
        }
    }

    public Quaternion Rotation
    {
        get => Transform.rotation;
        set
        {
            NetworkServer.UnSpawn(GameObject);
            Transform.rotation = value;
            NetworkServer.Spawn(GameObject);
        }
    }

    public Player? KnownUser
    {
        get => Controller._knownUser.GetPlayer();
        set => Controller._knownUser = value?.ReferenceHub;
    }

    public bool Activated
    {
        get => Status == WorkstationStatus.Online;
        set => Status = value ? WorkstationStatus.Online : WorkstationStatus.Offline;
    }

    public WorkstationStatus Status
    {
        get => (WorkstationStatus)Controller.Status;
        set
        {
            Controller.NetworkStatus = (byte)value;

            switch (value)
            {
                case WorkstationStatus.Offline:
                {
                    Controller._serverStopwatch.Stop();
                    break;
                }
                case WorkstationStatus.PoweringUp:
                {
                    Controller._serverStopwatch.Restart();
                    break;
                }
                default:
                {
                    Controller._serverStopwatch.Restart();
                    break;
                }
            } // end switch
        } // end field_set
    } // end field
}