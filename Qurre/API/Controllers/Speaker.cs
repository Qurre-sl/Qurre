using System;
using AdminToys;
using Footprinting;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Addons;
using Qurre.API.Addons.Audio;
using UnityEngine;
using VoiceChat.Playbacks;

namespace Qurre.API.Controllers;

// TODO: разобраться с лимитом в 4 штуки. Может быть сделать SpeakerPool?
[PublicAPI]
public class Speaker
{
    public SpeakerToy Base { get; }

    public SpeakerToyPlaybackBase Playback => Base.Playback;

    public AudioPlayerSpeaker ApiPlayer { get; }
    
    public Vector3 Position
    {
        get => Base.transform.position;
        set
        {
            Base.transform.position = value;
            Base.NetworkPosition = value;
        }
    }

    public Quaternion RotationQuaternion
    {
        get => Base.transform.rotation;
        set
        {
            Base.transform.rotation = value;
            Base.NetworkRotation = value;
        }
    }

    public Vector3 RotationEuler
    {
        get => Base.transform.rotation.eulerAngles;
        set
        {
            Quaternion quaternion = Quaternion.Euler(value);
            Base.transform.localRotation = quaternion;
            Base.NetworkRotation = quaternion;
        }
    }

    public Vector3 Scale
    {
        get => Base.transform.localScale;
        set
        {
            Base.transform.localScale = value;
            Base.NetworkScale = Base.transform.lossyScale;
        }
    }

    public Vector3 GlobalScale => Base.transform.lossyScale;

    public byte MovementSmoothing
    {
        get => Base.NetworkMovementSmoothing;
        set => Base.NetworkMovementSmoothing = value;
    }

    public bool IsStatic
    {
        get => Base.NetworkIsStatic;
        set
        {
            if (value)
            {
                Base.NetworkPosition = Base.transform.position;
                Base.NetworkRotation = Base.transform.rotation;
                Base.NetworkScale = Base.transform.lossyScale;
            }

            Base.NetworkIsSpatial = value;
        }
    }

    public byte ControllerId
    {
        get => Base.NetworkControllerId;
        set => Base.NetworkControllerId = value;
    }

    public bool IsSpatial
    {
        get => Base.NetworkIsSpatial;
        set => Base.NetworkIsSpatial = value;
    }

    public float Volume
    {
        get => Base.NetworkVolume;
        set => Base.NetworkVolume = value;
    }

    public float MinDistance
    {
        get => Base.NetworkMinDistance;
        set => Base.NetworkMinDistance = value;
    }

    public float MaxDistance
    {
        get => Base.NetworkMaxDistance;
        set => Base.NetworkMaxDistance = value;
    }

    public Speaker(SpeakerToy speakerToy)
    {
        Base = speakerToy;
        ApiPlayer = new AudioPlayerSpeaker(speakerToy);
    }

    public Speaker(Vector3 position, byte controllerId = 0, bool isSpatial = true,
        float volume = 1.0F, float minDistance = 1.0F, float maxDistance = 15.0F)
    {
        if (Prefabs.Speaker == null)
            throw new NullReferenceException(nameof(Prefabs.Speaker));

        if (!Prefabs.Speaker.TryGetComponent<SpeakerToy>(out SpeakerToy? speakerToy))
            throw new NullReferenceException("SpeakerToy not found");

        Base = speakerToy;
        Base.SpawnerFootprint = new Footprint(Server.Host.ReferenceHub);
        NetworkServer.Spawn(Base.gameObject);
        
        ApiPlayer = new AudioPlayerSpeaker(speakerToy);
        Position = position;

        ControllerId = controllerId;
        IsSpatial = isSpatial;
        Volume = volume;
        MinDistance = minDistance;
        MaxDistance = maxDistance;
    }
}