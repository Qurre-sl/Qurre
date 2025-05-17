using JetBrains.Annotations;
using MEC;
using Qurre.API.Controllers.Structs;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Broadcast(Player player, string message, ushort time)
{
    private string _msg = message;

    public float DisplayTime { get; internal set; } = float.MinValue;
    public ushort Time { get; } = time;
    public bool Active { get; private set; }

    public string Message
    {
        get => _msg;
        set
        {
            if (value == _msg)
                return;

            _msg = value;

            if (Active)
                Update();
        }
    }

    public void Start()
    {
        if (Active)
            return;

        if (player.Broadcasts.FirstOrDefault() != this)
            return;

        Active = true;
        DisplayTime = UnityEngine.Time.time;

        BcComponent.Component.TargetAddElement(player.Connection, Message, Time,
            global::Broadcast.BroadcastFlags.Normal);
        Timing.CallDelayed(Time, End);
    }

    public void Update()
    {
        float time = Time - (UnityEngine.Time.time - DisplayTime) + 1;
        BcComponent.Component.TargetClearElements(player.Connection);
        BcComponent.Component.TargetAddElement(player.Connection, Message, (ushort)time,
            global::Broadcast.BroadcastFlags.Normal);
    }

    public void End()
    {
        if (!Active)
            return;

        Active = false;
        player.Broadcasts.Remove(this);
        BcComponent.Component.TargetClearElements(player.Connection);

        if (player.Broadcasts.Any())
            player.Broadcasts.First().Start();
    }

    internal void SilentEnd()
    {
        Active = false;
        BcComponent.Component.TargetClearElements(player.Connection);
    }
}