using JetBrains.Annotations;
using Qurre.API.World;
using Respawning;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Cassie(string message, bool makeHold, bool makeNoise)
{
    public static bool Lock { get; set; }

    public string Message { get; set; } = message;
    public bool Hold { get; set; } = makeHold;
    public bool Noise { get; set; } = makeNoise;
    public bool Active { get; private set; }

    public void Send()
    {
        if (Active) return;
        Active = true;
        RespawnEffectsController.PlayCassieAnnouncement(Message, Hold, Noise);
    }

    public static void Send(string msg, bool makeHold = false, bool makeNoise = false, bool instant = false)
    {
        Map.Cassies.Add(new Cassie(msg, makeHold, makeNoise), instant);
    }

    internal static void End()
    {
        if (Map.Cassies.Count > 0)
            Map.Cassies[0].Send();
    }
}