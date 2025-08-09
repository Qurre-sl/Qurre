using JetBrains.Annotations;
using LightContainmentZoneDecontamination;

namespace Qurre.API.World;

[PublicAPI]
public static class Decontamination
{
    public static DecontaminationController Controller => DecontaminationController.Singleton;
    public static bool Begun => Controller.IsDecontaminating;
    public static bool InProgress => Controller._decontaminationBegun;

    public static DecontaminationController.DecontaminationStatus Status
    {
        get => Controller._decontaminationOverride;
        set => Controller._decontaminationOverride = value;
    }

    public static bool Locked
    {
        get => Controller._stopUpdating;
        set => Controller._stopUpdating = value;
    }

    public static void InstantStart()
    {
        Controller.FinishDecontamination();
    }
}