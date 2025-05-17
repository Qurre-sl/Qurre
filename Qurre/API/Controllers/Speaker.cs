using System;
using AdminToys;
using Footprinting;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Addons;
using Qurre.API.Addons.Audio;
using Qurre.API.Controllers.Components;
using Qurre.API.World;
using UnityEngine;
using VoiceChat.Playbacks;

namespace Qurre.API.Controllers;

// TODO: разобраться с лимитом в 4 штуки. Может быть сделать SpeakerPool?
[PublicAPI]
public class Speaker : AdminToy<SpeakerToy>
{
    public Speaker(SpeakerToy speakerToy)
    {
        Base = speakerToy;
        ApiPlayer = new AudioPlayerSpeaker(speakerToy);

        Map.Speakers.Add(this);
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

        Map.Speakers.Add(this);
    }

    public SpeakerToyPlaybackBase Playback => Base.Playback;

    public AudioPlayerSpeaker ApiPlayer { get; }

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

    public override void Destroy()
    {
        ApiPlayer.DestroySelf();
        NetworkServer.Destroy(Base.gameObject);
        Map.Speakers.Remove(this);
    }
}