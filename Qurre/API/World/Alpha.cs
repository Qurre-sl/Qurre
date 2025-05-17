using JetBrains.Annotations;
using LabApi.Features.Wrappers;

namespace Qurre.API.World;

[PublicAPI]
public static class Alpha
{
    public static AlphaWarheadController? Controller => Warhead.BaseController;
    public static AlphaWarheadNukesitePanel? InnerPanel => Warhead.BaseNukesitePanel;
    public static AlphaWarheadOutsitePanel? OutsidePanel => Warhead.BaseOutsidePanel;

    public static bool Detonated => Warhead.IsDetonated;
    public static bool Active => Warhead.IsDetonationInProgress;
    public static bool DeadManAllow { get; set; } = true; // TODO

    public static bool Authorized
    {
        get => Warhead.IsAuthorized;
        set => Warhead.IsAuthorized = value;
    }

    public static bool Locked
    {
        get => Warhead.IsLocked;
        set => Warhead.IsLocked = value;
    }

    public static bool InnerPanelEnabled
    {
        get => Warhead.LeverStatus;
        set => Warhead.LeverStatus = value;
    }

    public static float TimeToDetonation
    {
        get => Warhead.DetonationTime;
        set => Warhead.DetonationTime = value;
    }

    public static double Cooldown
    {
        get => Warhead.CooldownTime;
        set => Warhead.CooldownTime = value;
    }

    public static void Start(bool isAutomatic = false, bool suppressSubtitles = false)
    {
        Warhead.Start(isAutomatic, suppressSubtitles);
    }

    public static void InstantPrepare()
    {
        Controller?.InstantPrepare();
    }

    public static void Stop()
    {
        Warhead.Stop();
    }

    public static void Detonate()
    {
        Warhead.Detonate();
    }

    public static void Shake()
    {
        Warhead.Shake();
    }
}