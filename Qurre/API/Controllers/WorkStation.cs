using System;
using InventorySystem.Items.Firearms.Attachments;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Addons;
using Qurre.API.Controllers.Components;
using Qurre.API.Objects;
using Qurre.API.World;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class WorkStation : NetTransform
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

    public override GameObject GameObject => Controller.gameObject;
    public string Name => GameObject.name;

    public Player? KnownUser
    {
        get => Controller.KnownUser.GetPlayer();
        set => Controller.KnownUser = value?.ReferenceHub;
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
                        Controller.ServerStopwatch.Stop();
                        break;
                    }
                case WorkstationStatus.PoweringUp:
                    {
                        Controller.ServerStopwatch.Restart();
                        break;
                    }
                default:
                    {
                        Controller.ServerStopwatch.Restart();
                        break;
                    }
            } // end switch
        } // end field_set
    } // end field

    public override void Destroy()
    {
        NetworkServer.Destroy(GameObject);
        Map.WorkStations.Remove(this);
    }
}